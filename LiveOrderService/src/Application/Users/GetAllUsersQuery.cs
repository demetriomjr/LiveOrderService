using Application.DTOs;
using Application.Repositories;
using MediatR;

namespace LiveOrderService.src.Application.Users
{
    public record GetAllUsersQuery : IRequest<IEnumerable<UserResponseDto>>;

    public class GetAllUsersQueryHandler(IUserRepository _userRepository) : IRequestHandler<GetAllUsersQuery, IEnumerable<UserResponseDto>>
    {
        public async Task<IEnumerable<UserResponseDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserResponseDto(u));
        }
    }
}