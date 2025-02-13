using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.Application.Users
{
    public record DeleteUserCommand(uint Id) : IRequest<int>;

    public class DeleteUserCommandHandler(IUserRepository _userRepository) : IRequestHandler<DeleteUserCommand, int>
    {
        public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if(user is null) return 0;
            user.Status = Domain.Interfaces.IBaseModel.StatusOptions.DELETED;
            return await _userRepository.UpdateAsync(user);
        }
    }
}