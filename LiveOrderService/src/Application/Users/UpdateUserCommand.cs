using Application.DTOs;
using Application.Repositories;
using MediatR;

namespace Application.Users
{
    public record UpdateUserCommand(UserRequestDto user) : IRequest<int>;

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, int>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null) return 0;

            return await _userRepository.UpdateAsync(request.user.ToModel());
        }
    }
}