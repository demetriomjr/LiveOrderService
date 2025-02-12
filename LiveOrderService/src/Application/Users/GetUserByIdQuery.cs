using Application.DTOs;
using Application.Repositories;
using MediatR;

namespace Application.Users
{
    public record GetUserByIdQuery(uint Id) : IRequest<UserDto?>;

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            return new UserDto(user);
        }
    }
}