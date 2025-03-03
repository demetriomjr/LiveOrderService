using LanguageExt.Common;
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

            return result.Match(
                createdUser => new Result<UserResponseDto>(new UserResponseDto(createdUser)),
                error => new Result<UserResponseDto>(error)
            );
        }
    }
}