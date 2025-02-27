using CSharpFunctionalExtensions;
using LiveOrderService.Domain.Users;
using OneOf;

namespace LiveOrderService.Application.Repositories
{
    public interface IUserRepository
    {
        Task<OneOf<IEnumerable<User>, string>> GetAllAsync();

        Task<OneOf<User, string>> GetByIdAsync(uint id);

        Task<OneOf<User, string>> GetByUsernameAsync(string username);

        Task<OneOf<User, string>> AddAsync(User user);

        Task<Result<bool>> UpdateAsync(User user);
        
        Task<Result<bool>> DeleteAsync(uint id);
    }
}