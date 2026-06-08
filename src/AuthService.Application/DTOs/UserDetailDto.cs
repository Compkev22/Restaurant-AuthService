namespace AuthService.Application.DTOs;

public class UserDetailsDto
{
    public string Id { get; set; } = string.Empty;

    // Campos de identidad completos (necesarios para el perfil del cliente)
    public string UserName { get; set; } = string.Empty;
    public string UserSurname { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    // Campos de estado y rol
    public string Role { get; set; } = string.Empty;
    public string UserStatus { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public DateTime UserCreatedAt { get; set; }

    // Campos de perfil
    public string ProfilePicture { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? BranchId { get; set; }
}