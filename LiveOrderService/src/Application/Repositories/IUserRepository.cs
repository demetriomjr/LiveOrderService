using LanguageExt.Common;
using LiveOrderService.Domain.Users;

namespace LiveOrderService.Application.Repositories
{
    public interface IUserRepository
    {
        Task<Result<IEnumerable<User>>> GetAllAsync();

        Task<Result<User>> GetByIdAsync(uint id);

        Task<Result<User>> GetByUsernameAsync(string username);

        Task<Result<User>> AddAsync(User user);

        Task<Result<bool>> UpdateAsync(User user);
        
        Task<Result<bool>> DeleteAsync(uint id);
    }
}