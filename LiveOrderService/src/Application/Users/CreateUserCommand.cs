using LiveOrderService.Application.DTOs;
using LiveOrderService.Application.Repositories;
using LiveOrderService.Domain.Users;
using MediatR;

namespace LiveOrderService.Application.Users
{
    public record CreateUserCommand(string Username, string Password) : IRequest<UserResponseDto?>;

    public class CreateUserCommandHandler(IUserRepository _userRepository) : IRequestHandler<CreateUserCommand, UserResponseDto?>
    {
        public async Task<UserResponseDto?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User(request.Username, request.Password);
            var result = await _userRepository.AddAsync(user);
            return result is null ? null : new UserResponseDto(result);
        }
    }
}