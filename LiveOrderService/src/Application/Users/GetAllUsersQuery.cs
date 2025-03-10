using LanguageExt.Common;
using LiveOrderService.Application.DTOs;
using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.src.Application.Users
{
    public record GetAllUsersQuery : IRequest<Result<IEnumerable<UserResponseDto>>>;

    public class GetAllUsersQueryHandler(IUserRepository _userRepository) : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserResponseDto>>>
    {
        public async Task<Result<IEnumerable<UserResponseDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetAllAsync();

            return result.Match(
                users => new Result<IEnumerable<UserResponseDto>>(users.Select(user => new UserResponseDto(user))),
                error => new Result<IEnumerable<UserResponseDto>>(error)
            );
        }
    }
}