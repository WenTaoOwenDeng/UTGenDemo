namespace UTGenDemo.Service.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
    Task<bool> SendWelcomeEmailAsync(string userEmail, string userName);
    Task<bool> SendPasswordResetEmailAsync(string userEmail, string resetToken);
}
