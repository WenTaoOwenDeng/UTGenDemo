using UTGenDemo.Repository.Interfaces;
using UTGenDemo.Repository.Models;

namespace UTGenDemo.Repository.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users;

    public InMemoryUserRepository()
    {
        _users = new List<User>
        {
            new User
            {
                Id = "1",
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                IsActive = true
            },
            new User
            {
                Id = "2",
                Email = "jane.smith@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                IsActive = true
            },
            new User
            {
                Id = "3",
                Email = "inactive.user@example.com",
                FirstName = "Inactive",
                LastName = "User",
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                IsActive = false
            }
        };
    }

    public Task<User?> GetUserByIdAsync(string userId)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        return Task.FromResult(user);
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        var activeUsers = _users.Where(u => u.IsActive);
        return Task.FromResult(activeUsers);
    }

    public Task<User> CreateUserAsync(User user)
    {
        // Generate a new ID
        var maxId = _users.Count > 0 ? _users.Max(u => int.Parse(u.Id)) : 0;
        user.Id = (maxId + 1).ToString();
        user.CreatedAt = DateTime.UtcNow;
        
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<User> UpdateUserAsync(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.IsActive = user.IsActive;
            return Task.FromResult(existingUser);
        }
        
        throw new InvalidOperationException($"User with ID {user.Id} not found");
    }

    public Task<bool> DeleteUserAsync(string userId)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            _users.Remove(user);
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }

    public Task<bool> ExistsAsync(string userId)
    {
        var exists = _users.Any(u => u.Id == userId);
        return Task.FromResult(exists);
    }
}