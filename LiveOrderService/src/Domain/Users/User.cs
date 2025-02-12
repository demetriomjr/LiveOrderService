
using System.Security.Cryptography;
using System.Text;
using Domain.Interfaces;

namespace Domain.Users
{
    public class User : IBaseModel
    {
        public uint Id { get; set; } = 0;
        public IBaseModel.StatusOptions Status { get; set; } = IBaseModel.StatusOptions.ACTIVE;
        public string Name { get; set; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public byte[] Password { get; init; } = [];
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public DateTime? DeletedAt { get; set; } = null;

        public User(string username, string password)
        {
            Username = username;
            Password = HashPassword(password);
        }

        private byte[] CombineWithSalt(byte[] data, byte[] salt)
        {
            var combined = new byte[data.Length + salt.Length];
            Buffer.BlockCopy(data, 0, combined, 0, data.Length);
            Buffer.BlockCopy(salt, 0, combined, data.Length, salt.Length);
            return combined;
        }

        private byte[] HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = CombineWithSalt(passwordBytes, salt);
            var hashedBytes = sha256.ComputeHash(saltedPassword);
            
            return CombineWithSalt(hashedBytes, salt);
        }

        public bool VerifyPassword(string password)
        {
            var salt = new byte[16];
            Buffer.BlockCopy(Password, Password.Length - salt.Length, salt, 0, salt.Length);
            
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = CombineWithSalt(passwordBytes, salt);
            
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(saltedPassword);
            
            for (var i = 0; i < hashedBytes.Length; i++)
            {
                if (hashedBytes[i] != Password[i])
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
