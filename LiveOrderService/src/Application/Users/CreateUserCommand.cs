using CSharpFunctionalExtensions;
using LiveOrderService.Application.DTOs;
using LiveOrderService.Application.Repositories;
using LiveOrderService.Domain.Users;
using MediatR;

namespace LiveOrderService.Application.Users
{
    public record CreateUserCommand(string Username, string Password) : IRequest<Result<UserResponseDto>>;

    public class CreateUserCommandHandler(IUserRepository _userRepository) : IRequestHandler<CreateUserCommand, Result<UserResponseDto>>
    {
        public async Task<Result<UserResponseDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User(request.Username, request.Password);
            var result = await _userRepository.AddAsync(user);
            
            if(result.Value is string error)
                return Result.Failure<UserResponseDto>(error);
            
            if(result.Value is User createdUser)
                return Result.Success(new UserResponseDto(createdUser));
            
            return Result.Failure<UserResponseDto>("An error occurred while creating the user.");
        }
    }
}