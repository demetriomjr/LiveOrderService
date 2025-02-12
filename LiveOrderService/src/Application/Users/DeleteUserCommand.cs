using Application.Repositories;
using MediatR;

namespace Application.Users
{
    public record DeleteUserCommand(uint Id) : IRequest<int>;

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, int>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository context)
        {
            _userRepository = context;
        }

        public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if(user is null) return 0;
            user.Status = Domain.Interfaces.IBaseModel.StatusOptions.DELETED;
            return await _userRepository.UpdateAsync(user);
        }
    }
}