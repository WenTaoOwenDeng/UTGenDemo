using UTGenDemo.Repository.Models;

namespace UTGenDemo.Service.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(string userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<bool> DeactivateUserAsync(string userId);
    Task<User> UpdateUserAsync(User user);
}