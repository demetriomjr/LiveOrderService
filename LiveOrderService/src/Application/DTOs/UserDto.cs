using Domain.Users;

namespace Application.DTOs 
{
    public class UserDto
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }

        public UserDto(uint id, string name, string username) => (Id, Name, Username) = (id, name, username);
        public UserDto(User user) => (Id, Name, Username) = (user.Id, user.Name, user.Username);
    }
}