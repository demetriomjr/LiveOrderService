using LanguageExt.Common;
using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.Application.Users
{
    public record DeleteUserCommand(uint Id) : IRequest<Result<bool>>;

    public class DeleteUserCommandHandler(IUserRepository _userRepository) : IRequestHandler<DeleteUserCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.DeleteAsync(request.Id);
                
            result.Match(
                _ => new Result<bool>(true),
                ex => new Result<bool>(ex)
            );

            return new Result<bool>(new Exception("An error has occured while trying to delete the user"));
        }
    }
}