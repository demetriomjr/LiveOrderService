using Domain.Users;

namespace Application.DTOs
{
    public class UserRequestDto
    {
        public uint Id {get; set;} = 0;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public User ToModel()
        {
            return new User(this.Username, this.Password);
        }
    }
}