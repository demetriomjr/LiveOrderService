using CSharpFunctionalExtensions;
using LiveOrderService.Application.DTOs;
using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.Application.Users
{
    public record GetUserByUsernameQuery(string Username) : IRequest<Result<UserResponseDto>>;

    public class GetUserByUsernameQueryHandler(IUserRepository _userRepository) : IRequestHandler<GetUserByUsernameQuery, Result<UserResponseDto>>
    {
        public async Task<Result<UserResponseDto>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetByUsernameAsync(request.Username);
            
            if(result.Value is string error)
                return Result.Failure<UserResponseDto>(error);
            
            if(result.Value is UserResponseDto user)
                return Result.Success(user);
            
            return Result.Failure<UserResponseDto>("An error occurred while fetching the user.");
        }
    }
}