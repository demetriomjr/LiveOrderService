
namespace LiveOrderService.Application.DTOs
{
    public record struct AuthResponse(string Token, string RefreshToken);
}