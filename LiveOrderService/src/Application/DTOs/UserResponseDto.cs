using Domain.Users;

namespace Application.DTOs 
{
    public class UserResponseDto
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }

        public UserResponseDto(uint id, string name, string username) => (Id, Name, Username) = (id, name, username);
        public UserResponseDto(User user) => (Id, Name, Username) = (user.Id, user.Name, user.Username);
    }
}