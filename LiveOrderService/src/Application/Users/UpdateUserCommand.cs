using CSharpFunctionalExtensions;
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
            
            if(FetchedUser.Value is string error)
                return Result.Failure<bool>(error);

            if(FetchedUser.Value is User user)
            {
                if(!string.IsNullOrEmpty(request.username))
                    user.Username = request.username;
                if(!string.IsNullOrEmpty(request.password))
                    user.SetNewPassword(request.password);

                var result = await _userRepository.UpdateAsync(user);

                if(result.IsSuccess)
                    return Result.Success(true);
                
                if(result.IsFailure)
                    return Result.Failure<bool>(result.Error);
                
                return Result.Failure<bool>("An error occurred while updating the user.");
            }
            
            return Result.Failure<bool>("An error occurred while updating the user.");
        }
    }
}