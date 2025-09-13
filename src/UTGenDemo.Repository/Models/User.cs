namespace UTGenDemo.Repository.Models;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    public bool IsEmailValid()
    {
        return !string.IsNullOrWhiteSpace(Email) && Email.Contains('@');
    }
}
