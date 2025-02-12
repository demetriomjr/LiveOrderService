using Application.DTOs;
using Application.Repositories;
using MediatR;

namespace Application.Users
{
    public record CreateUserCommand(UserRequestDto user) : IRequest<UserResponseDto?>;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponseDto?>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponseDto?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = request.user.ToModel();
            var result = await _userRepository.AddAsync(user);
            return new UserResponseDto(result);
        }
    }
}