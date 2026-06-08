namespace AuthService.Application.DTOs;

public class AuthResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    // UserDetails enriquecido para que el frontend del cliente tenga todos los campos
    public UserDetailsDto UserDetails { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
}