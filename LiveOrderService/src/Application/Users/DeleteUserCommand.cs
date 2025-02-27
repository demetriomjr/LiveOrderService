using CSharpFunctionalExtensions;
using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.Application.Users
{
    public record DeleteUserCommand(uint Id) : IRequest<Result<bool>>;

    public class DeleteUserCommandHandler(IUserRepository _userRepository) : IRequestHandler<DeleteUserCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var deleteResult = await _userRepository.DeleteAsync(request.Id);
                
            if(deleteResult.IsFailure)
                return Result.Failure<bool>(deleteResult.Error);        

            if(deleteResult.IsSuccess)
                return Result.Success(true);
            
            return Result.Failure<bool>("An error occurred while deleting the user");
        }
    }
}