using UTGenDemo.Service.Interfaces;

namespace UTGenDemo.Service.Services;

public class MockEmailService : IEmailService
{
    public Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        // Mock implementation - just log the email attempt
        Console.WriteLine($"[MOCK EMAIL] To: {to}, Subject: {subject}, Body: {body}");
        
        // Simulate successful email sending
        return Task.FromResult(true);
    }

    public Task<bool> SendWelcomeEmailAsync(string userEmail, string userName)
    {
        var subject = "Welcome!";
        var body = $"Hello {userName}, welcome to our platform!";
        
        Console.WriteLine($"[MOCK WELCOME EMAIL] To: {userEmail}, Subject: {subject}, Body: {body}");
        
        return Task.FromResult(true);
    }

    public Task<bool> SendPasswordResetEmailAsync(string userEmail, string resetToken)
    {
        var subject = "Password Reset Request";
        var body = $"Click here to reset your password. Token: {resetToken}";
        
        Console.WriteLine($"[MOCK PASSWORD RESET EMAIL] To: {userEmail}, Subject: {subject}, Body: {body}");
        
        return Task.FromResult(true);
    }
}