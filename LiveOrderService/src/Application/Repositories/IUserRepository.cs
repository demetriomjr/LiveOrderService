using Domain.Users;

namespace Application.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetByIdAsync(uint id);

        Task<User> GetByUsernameAsync(string username);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(uint id);
    }
}