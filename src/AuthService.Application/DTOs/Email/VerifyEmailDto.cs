namespace AuthService.Application.DTOs.Email;
using System.ComponentModel.DataAnnotations;

public class VerifyEmailDto
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El token de verificación es obligatorio")]
    public string Token { get; set; } = string.Empty;
}