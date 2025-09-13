using UTGenDemo.Service.Interfaces;
using UTGenDemo.Repository.Models;
using UTGenDemo.Repository.Interfaces;

namespace UTGenDemo.Service.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public UserService(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        }

        return await _userRepository.GetUserByIdAsync(userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return await _userRepository.GetUserByEmailAsync(email);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (!user.IsEmailValid())
        {
            throw new ArgumentException("Invalid email address", nameof(user));
        }

        // Check if user already exists
        var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email {user.Email} already exists");
        }

        user.Id = Guid.NewGuid().ToString();
        user.CreatedAt = DateTime.UtcNow;

        var createdUser = await _userRepository.CreateUserAsync(user);
        
        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);

        return createdUser;
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _userRepository.GetActiveUsersAsync();
    }

    public async Task<bool> DeactivateUserAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.IsActive = false;
        await _userRepository.UpdateUserAsync(user);
        return true;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (string.IsNullOrWhiteSpace(user.Id))
        {
            throw new ArgumentException("User ID is required for updates", nameof(user));
        }

        var existingUser = await _userRepository.GetUserByIdAsync(user.Id);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with ID {user.Id} not found");
        }

        return await _userRepository.UpdateUserAsync(user);
    }
}
