using LanguageExt.Common;
using LiveOrderService.Application.DTOs;
using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.Application.Users
{
    public record GetUserByUsernameQuery(string Username) : IRequest<Result<UserResponseDto>>;

    public class GetUserByUsernameQueryHandler(IUserRepository _userRepository) : IRequestHandler<GetUserByUsernameQuery, Result<UserResponseDto>>
    {
        public async Task<Result<UserResponseDto>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetByUsernameAsync(request.Username);

            return result.Match(
                user => new Result<UserResponseDto>(new UserResponseDto(user)),
                error => new Result<UserResponseDto>(error)
            );
        }
    }
}