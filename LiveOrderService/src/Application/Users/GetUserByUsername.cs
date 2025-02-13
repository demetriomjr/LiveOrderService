using LiveOrderService.Application.DTOs;
using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.Application.Users
{
    public record GetUserByUsernameQuery(string Username) : IRequest<UserResponseDto?>;

    public class GetUserByUsernameQueryHandler(IUserRepository _userRepository) : IRequestHandler<GetUserByUsernameQuery, UserResponseDto?>
    {
        public async Task<UserResponseDto?> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            return new UserResponseDto(user);
        }
    }
}