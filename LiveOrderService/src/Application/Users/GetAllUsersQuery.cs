using CSharpFunctionalExtensions;
using LiveOrderService.Application.DTOs;
using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.src.Application.Users
{
    public record GetAllUsersQuery : IRequest<Result<IEnumerable<UserResponseDto>>>;

    public class GetAllUsersQueryHandler(IUserRepository _userRepository) : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserResponseDto>>>
    {
        public async Task<Result<IEnumerable<UserResponseDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetAllAsync();
            if(result.Value is IEnumerable<UserResponseDto> users)
                return Result.Success(users);

            if(result.Value is string error)
                return Result.Failure<IEnumerable<UserResponseDto>>(error);

            return Result.Failure<IEnumerable<UserResponseDto>>("An error occurred while fetching all users.");
        }
    }
}