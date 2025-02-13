using Application.DTOs;
using Application.Repositories;
using MediatR;

namespace Application.Users
{
    public record GetUserByIdQuery(uint Id) : IRequest<UserResponseDto?>;

    public class GetUserByIdQueryHandler(IUserRepository _userRepository) : IRequestHandler<GetUserByIdQuery, UserResponseDto?>
    {
        public async Task<UserResponseDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            return new UserResponseDto(user);
        }
    }
}