using Application.DTOs;
using Application.Repositories;
using MediatR;

namespace Application.Users
{
    public record GetUserByUsernameQuery(string Username) : IRequest<UserDto?>;

    public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, UserDto?>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByUsernameQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            return new UserDto(user);
        }
    }
}