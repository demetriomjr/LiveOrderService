using LanguageExt.Common;
using LiveOrderService.Application.Repositories;
using LiveOrderService.Domain.Users;
using MediatR;

namespace LiveOrderService.Application.Users
{
    public record UpdateUserCommand(uint Id, string username, string password) : IRequest<Result<bool>>;

    public class UpdateUserCommandHandler(IUserRepository _userRepository) : IRequestHandler<UpdateUserCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var FetchedUser = await _userRepository.GetByIdAsync(request.Id);
            
            await FetchedUser.Match(
                async (user) =>
                {
                    if(!string.IsNullOrEmpty(request.username))
                        user.Username = request.username;
                    if(!string.IsNullOrEmpty(request.password))
                        user.SetNewPassword(request.password);

                    var result = await _userRepository.UpdateAsync(user);

                    result.Match(
                        _ => new Result<bool>(true),
                        ex => new Result<bool>(ex)
                    );

                    return new Result<bool>(new Exception("An error occurred while updating the user."));
                },
                error => Task.FromResult(new Result<bool>(error))
            );
           
            return new Result<bool>(new Exception("An error occurred while updating the user."));
        }
    }
}