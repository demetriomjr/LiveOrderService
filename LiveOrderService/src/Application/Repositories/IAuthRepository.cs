using LanguageExt.Common;
using LiveOrderService.Application.DTOs;

namespace LiveOrderService.Application.Repositories
{
    public interface IAuthRepository
    {
        Task<Result<AuthResponse>> AuthenticateUserAsync(string username, string password, string personalKey);
        Task<Result<string>> ValidateTokenAsync(string token, string personalKey);
        Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, string personalKey);
    }
}