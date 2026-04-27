using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs.Email;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "El token de recuperación es obligatorio.")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [MinLength(8, ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres.")]
    public string NewPassword { get; set; } = string.Empty;
}