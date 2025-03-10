using LanguageExt.Common;
using LiveOrderService.Application.DTOs;
using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.Application.Authentication
{
    public record RefreshTokenQuery(string RefreshToken, string PersonalKey) : IRequest<Result<AuthResponse>>;

    public class RefreshTokenQueryHandler(IAuthRepository _authRepository) : IRequestHandler<RefreshTokenQuery, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var result = await _authRepository.RefreshTokenAsync(request.RefreshToken, request.PersonalKey);

            result.Match(
                authResponse => authResponse,
                error => new Result<AuthResponse>(error)
            );

            return new Result<AuthResponse>(new Exception("Internal Server Error"));
        }
    }
}