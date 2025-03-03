using LanguageExt.Common;
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
            
            result.Match(
                user => new Result<UserResponseDto>(new UserResponseDto(user)),
                error => new Result<UserResponseDto>(error)
            );
            
            return new Result<UserResponseDto>(new Exception("Error getting user by id"));
        }
    }
}