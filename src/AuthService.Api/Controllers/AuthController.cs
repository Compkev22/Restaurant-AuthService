using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Email;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Inicia sesión en el sistema del restaurante.
    /// </summary>
    /// <remarks>
    /// Permite el acceso a empleados y clientes. Devuelve un JWT válido por 30 minutos.
    /// </remarks>
    /// <param name="loginDto">Credenciales de acceso (Email/Username y Password).</param>
    /// <response code="200">Login exitoso. Devuelve el Token y datos del usuario.</response>
    /// <response code="401">Credenciales inválidas.</response>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromForm] LoginDto loginDto)
    {
        var result = await authService.LoginAsync(loginDto);
        return Ok(result);
    }

    /// <summary>
    /// Registra un nuevo cliente en el restaurante.
    /// </summary>
    /// <remarks>
    /// Por defecto asigna el rol 'CLIENT_ROLE'. Se envía un correo de verificación automáticamente.
    /// </remarks>
    /// <param name="registerDto">Datos del nuevo usuario (incluye imagen de perfil opcional).</param>
    /// <response code="201">Usuario creado correctamente.</response>
    /// <response code="409">El correo o nombre de usuario ya está registrado.</response>
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromForm] RegisterDto registerDto)
    {
        var result = await authService.RegisterAsync(registerDto);
        return StatusCode(201, result);
    }

    /// <summary>
    /// Obtiene el perfil completo del usuario autenticado.
    /// </summary>
    /// <remarks>
    /// Requiere que el Token JWT sea enviado en el Header 'Authorization' como 'Bearer [token]'.
    /// </remarks>
    /// <response code="200">Retorna la información del perfil del usuario.</response>
    /// <response code="401">No autorizado (Token inválido o ausente).</response>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<object>> GetProfile()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        if (userIdClaim == null) return Unauthorized();

        var user = await authService.GetUserByIdAsync(userIdClaim.Value);
        return Ok(new { success = true, data = user });
    }

    /// <summary>
    /// Verifica el correo electrónico mediante un token.
    /// </summary>
    /// <param name="verifyEmailDto">Token recibido por correo.</param>
    [HttpPost("verify-email")]
    public async Task<ActionResult> VerifyEmail([FromForm] VerifyEmailDto verifyEmailDto)
    {
        var result = await authService.VerifyEmailAsync(verifyEmailDto);
        return Ok(result);
    }

    /// <summary>   
    /// Solicita un enlace para restablecer la contraseña.
    /// </summary>
    /// <param name="forgotPasswordDto">Correo del usuario que olvidó su clave.</param>
    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromForm] ForgotPasswordDto forgotPasswordDto)
    {
        var result = await authService.ForgotPasswordAsync(forgotPasswordDto);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el perfil de un usuario mediante su ID.
    /// </summary>
    /// <remarks>
    /// Útil para que los administradores busquen información de empleados o clientes específicos.
    /// </remarks>
    /// <param name="request">ID del usuario a buscar.</param>
    /// <response code="200">Retorna la información del perfil del usuario.</response>
    /// <response code="404">Usuario no encontrado.</response>
    [HttpPost("profile/by-id")]
    public async Task<ActionResult<object>> GetProfileById([FromForm] GetProfileByIdDto request)
    {
        if (string.IsNullOrEmpty(request.UserId))
        {
            return BadRequest(new { success = false, message = "El userId es requerido" });
        }

        var user = await authService.GetUserByIdAsync(request.UserId);
        
        if (user == null)
        {
            return NotFound(new { success = false, message = "Usuario no encontrado" });
        }

        return Ok(new { success = true, data = user });
    }

    /// <summary>
    /// Reenvía el correo de verificación.
    /// </summary>
    /// <remarks>
    /// Útil si el usuario no recibió el correo original o si el token expiró.
    /// </remarks>
    /// <param name="resendDto">Correo del usuario que necesita el reenvío.</param>
    /// <response code="200">Correo reenviado exitosamente.</response>
    [HttpPost("resend-verification")]
    public async Task<ActionResult> ResendVerification([FromForm] ResendVerificationDto resendDto)
    {
        var result = await authService.ResendVerificationEmailAsync(resendDto);
        return Ok(result);
    }

    /// <summary>
    /// Restablece la contraseña de un usuario.
    /// </summary>
    /// <remarks>
    /// Requiere el token enviado por correo en el paso de "forgot-password" y la nueva contraseña.
    /// </remarks>
    /// <param name="resetPasswordDto">Token de recuperación y nueva contraseña.</param>
    /// <response code="200">Contraseña actualizada correctamente.</response>
    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromForm] ResetPasswordDto resetPasswordDto)
    {
        var result = await authService.ResetPasswordAsync(resetPasswordDto);
        return Ok(result);
    }
}