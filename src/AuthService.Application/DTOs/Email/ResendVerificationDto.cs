using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs.Email;

public class ResendVerificationDto
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public string Email { get; set; } = string.Empty;
}