using LanguageExt.Common;
using LiveOrderService.Application.DTOs;
using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.Application.Authentication
{
    public record AuthenticateUserQuery(string Username, string Password, string PersonalKey) : IRequest<Result<AuthResponse>>;

    public class AuthenticateUserQueryHandler(IAuthRepository _authRepository) : IRequestHandler<AuthenticateUserQuery, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
        {
            var result = await _authRepository.AuthenticateUserAsync(request.Username, request.Password, request.PersonalKey);

            result.Match(
                authResponse => authResponse,
                error => new Result<AuthResponse>(error)
            );

            return new Result<AuthResponse>(new Exception("Internal Server Error"));
        }
    }
}