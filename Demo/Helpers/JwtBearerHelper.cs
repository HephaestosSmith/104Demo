using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Demo.Settings;

namespace Demo.Helpers
{
    public class JwtBearerHelper
    {
        public static TokenValidationParameters GetTokenValidationParameters(JwtSetting jwtSetting)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSetting.Secret));
            var tokenValidationParameters = GenerateTokenValidationParameters(jwtSetting);
            tokenValidationParameters.IssuerSigningKey = securityKey;

            return tokenValidationParameters;
        }

        public static TokenValidationParameters GetTokenValidationParametersAsymmetric(JwtSetting jwtSetting)
        {

            var tokenValidationParameters = GenerateTokenValidationParameters(jwtSetting);
            tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSetting.Secret));

            return tokenValidationParameters;
        }

        public string GenerateAccessToken(JwtSetting jwtSetting, IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSetting.Secret));

            var token = new JwtSecurityToken(
                   issuer: jwtSetting.Issuer,
                   claims: claims,
                   expires: DateTime.UtcNow.AddMinutes(jwtSetting.Expires),
                   signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                   );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var refrestToken = Convert.ToBase64String(randomNumber);
                return refrestToken;
            }
        }

        /// <summary>
        /// 驗證 access token，取得使用者身分識別
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="tokenValidationParameters"></param>
        /// <returns></returns>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken, TokenValidationParameters tokenValidationParameters)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                // 保留錯誤記錄區塊
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);
        }

        private static TokenValidationParameters GenerateTokenValidationParameters(JwtSetting jwtSetting)
        {
            return new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.NameIdentifier,
                LifetimeValidator = CustomLifetimeValidator,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidIssuer = jwtSetting.Issuer,
                ClockSkew = TimeSpan.Zero
            };
        }

        private static bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
        {
            if (param.ValidateLifetime)
            {
                if (expires.HasValue)
                {
                    return expires.Value > DateTime.UtcNow;
                }
                return false;
            }
            return true;
        }
    }
}