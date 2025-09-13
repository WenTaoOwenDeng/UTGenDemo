using UTGenDemo.Repository.Models;

namespace UTGenDemo.Repository.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(string userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> ExistsAsync(string userId);
}