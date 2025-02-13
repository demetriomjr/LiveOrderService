using LiveOrderService.Domain.Users;
using LiveOrderService.Application.Repositories;
using LiveOrderService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LiveOrderService.Infrastructure.Repositories
{
    public class UserRepository(PostgresDbContext _context, ILogger<UserRepository> _logger) : IUserRepository
    {
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(uint id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower().Trim()));
            return user;
        }

        public async Task<User?> AddAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user");
                return null;
            }
        }

        public async Task<int> UpdateAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    return 0;
                }

                _context.Users.Update(user);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return 0;
            }
        }

        public async Task<int> DeleteAsync(uint id)
        {
            try
            {
                var userToDelete = await _context.Users.FindAsync(id);
                if (userToDelete == null)
                {
                    return 0;
                }

                _context.Users.Remove(userToDelete);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return 0;
            }
        }
    }
}