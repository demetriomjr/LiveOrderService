using Domain.Users;

namespace Application.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetByIdAsync(uint id);

        Task<User> GetByUsernameAsync(string username);

        Task<User> AddAsync(User user);

        Task<int> UpdateAsync(User user);

        Task<int> DeleteAsync(uint id);
    }
}