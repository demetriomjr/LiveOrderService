using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.Application.Users
{
    public record UpdateUserCommand(uint Id, string username, string password) : IRequest<int>;

    public class UpdateUserCommandHandler(IUserRepository _userRepository) : IRequestHandler<UpdateUserCommand, int>
    {
        public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null) return 0;

            if(!string.IsNullOrEmpty(request.username))
                user.Username = request.username;
            if(!string.IsNullOrEmpty(request.password))
                user.SetNewPassword(request.password);

            return await _userRepository.UpdateAsync(user);
        }
    }
}