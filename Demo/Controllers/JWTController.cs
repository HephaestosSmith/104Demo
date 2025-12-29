using System;
using System.Linq;
using System.Threading.Tasks;
using Demo.Interface;
using Demo.Models.ApiResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JWTController : ControllerBase
    {
        private readonly ILogger<JWTController> _logger;
        private readonly ITokenService _tokenService;

        public JWTController(ILogger<JWTController> logger, ITokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
        }

        /*[HttpGet("GetJWTToken")]
        public async Task<ActionResult<TokenResponse>> GetJWTToken()
        {
            var demoUserId = Guid.NewGuid();
            var tokenResponse = await _tokenService.GenerateTokeResponseAsync(demoUserId);
            return Ok(tokenResponse);
        }*/

        /// <summary>
        /// 驗證 Authorization header 中的 Bearer token，回傳驗證結果
        /// </summary>
        /// <returns>驗證結果</returns>
        [HttpGet("CheckJWT")]
        [SwaggerOperation(Summary = "檢查 JWT", Description = "驗證 Authorization header 中的 Bearer token，回傳驗證結果")]
        public async Task<ActionResult<TokenResponse>> CheckJWT()
        {
            // 取得 Authorization header（可能是空或多值）
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                return Unauthorized(new { message = "Authorization header missing" });
            }

            const string bearerPrefix = "Bearer ";
            var token = authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)
                ? authHeader[bearerPrefix.Length..].Trim()
                : authHeader.Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new { message = "Bearer token missing" });
            }

            // 呼叫你的 token service 做驗證（假設 CheckAsync 接受 token 字串）
            var checkResult = await _tokenService.CheckAsync(token);
            if (checkResult == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            return Ok(checkResult);
        }
    }
}
