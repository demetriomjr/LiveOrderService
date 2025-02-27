using CSharpFunctionalExtensions;
using LiveOrderService.Application.DTOs;
using LiveOrderService.Application.Repositories;
using MediatR;

namespace LiveOrderService.Application.Users
{
    public record GetUserByIdQuery(uint Id) : IRequest<Result<UserResponseDto>>;

    public class GetUserByIdQueryHandler(IUserRepository _userRepository) : IRequestHandler<GetUserByIdQuery, Result<UserResponseDto>>
    {
        public async Task<Result<UserResponseDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetByIdAsync(request.Id);
            
            if(result.Value is string error)
                return Result.Failure<UserResponseDto>(error);

            if(result.Value is UserResponseDto user)
                return Result.Success(user);
            
            return Result.Failure<UserResponseDto>("Error getting user by id");
        }
    }
}