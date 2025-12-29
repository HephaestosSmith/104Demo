using Demo.Helpers;
using Demo.Models.ApiResponse;
using Demo.Models.Auth;
using Demo.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Demo.Interface;
using System.Security.Claims;

namespace Demo.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtBearerHelper _jwtBearerHelper;
        private readonly JwtSetting _jwtSetting;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IServiceProvider serviceProvider,
            JwtBearerHelper jwtBearerHelper,
            IOptionsSnapshot<JwtSetting> jwtSettingAccessor,
            ILogger<TokenService> logger)
        {
            _jwtBearerHelper = jwtBearerHelper;
            _jwtSetting = jwtSettingAccessor.Value;
            _tokenValidationParameters = serviceProvider.GetRequiredService<TokenValidationParameters>();
            _logger = logger;
        }

        public async Task<TokenResponse> GenerateTokeResponseAsync(Guid userId)
        {
            TokenResponse tokenResponse;


            var newAccessToken = _jwtBearerHelper.GenerateAccessToken(_jwtSetting, GetUserIdClaims(userId));

            tokenResponse = new TokenResponse
            {
                AccessToken = newAccessToken
            };

            return tokenResponse;
        }

        public async Task<VerificationTokenModel> CheckAsync(string tokenValue)
        {
            var result = new VerificationTokenModel();
            //取得JWT驗證身分
            var principal = _jwtBearerHelper.GetPrincipalFromExpiredToken(tokenValue, _tokenValidationParameters);

            if (principal == null)
            {
                result.IsVerification = false;
                result.ErrorCode = "401";
                result.ErrorMsg = "驗證失敗";
                return result;
            }

            //驗證成功
            result.IsVerification = true;
            return result;
        }

        private IEnumerable<Claim> GetUserIdClaims(Guid userId)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            return claims;
        }
    }
}
