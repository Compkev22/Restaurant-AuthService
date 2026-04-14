namespace AuthService.Application.DTOs;

public class UserResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserSurname { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string UserStatus { get; set; } = string.Empty;
    public string? BranchId { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime UserCreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Phone { get; set; }
}