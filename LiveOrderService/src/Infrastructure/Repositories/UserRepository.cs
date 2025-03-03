using LiveOrderService.Domain.Users;
using LiveOrderService.Application.Repositories;
using LiveOrderService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;


namespace LiveOrderService.Infrastructure.Repositories
{
    public class UserRepository(PostgresDbContext _context, ILogger<UserRepository> _logger) : IUserRepository
    {
        public async Task<Result<IEnumerable<User>>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<Result<User>> GetByIdAsync(uint id)
        {
           try
           {
                var user = await _context.Users.FindAsync(id);

                if(user is null)
                    return new Result<User>(new Exception($"User not found by this ID {id}"));
                
                return user;
           }
           catch (Exception ex)
           {
                _logger.LogError(ex, "Error getting user by ID", [id]);
                return new Result<User>(ex);  
           }
        }

        public async Task<Result<User>> GetByUsernameAsync(string username)
        {
            
            try
           {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower().Trim()));

                if(user is null)
                    return new Result<User>(new Exception($"User not found by this username {username}"));  
                
                return user;
           }
           catch (Exception ex)
           {
                _logger.LogError(ex, "Error getting user by username", [username]);
                return new Result<User>(ex);  
           }
        }

        public async Task<Result<User>> AddAsync(User user)
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
                return new Result<User>(ex);
            }
        }

        public async Task<Result<bool>> UpdateAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    return new Result<bool>(new Exception("User not found"));
                }

                _context.Users.Update(user);
                var result = await _context.SaveChangesAsync();

                if(result > 0)
                    return true;

                return new Result<bool>(new Exception("Error updating user"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return new Result<bool>(ex);
            }
        }

        public async Task<Result<bool>> DeleteAsync(uint id)
        {
            try
            {
                var userToDelete = await _context.Users.FindAsync(id);
                if (userToDelete == null)
                {
                    return new Result<bool>(new Exception("User not found"));
                }

                _context.Users.Remove(userToDelete);
                var result = await _context.SaveChangesAsync();

                if(result > 0)
                    return true;
                
                return new Result<bool>(new Exception("Error deleting user"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return new Result<bool>(ex);
            }
        }
    }
}