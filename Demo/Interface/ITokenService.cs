using Demo.Models.ApiResponse;
using Demo.Models.Auth;

namespace Demo.Interface
{
    public interface ITokenService
    {
        Task<TokenResponse> GenerateTokeResponseAsync(Guid userId);
        Task<VerificationTokenModel> CheckAsync(string tokenValue);
    }
}
