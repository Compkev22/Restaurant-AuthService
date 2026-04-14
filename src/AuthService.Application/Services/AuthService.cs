using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Application.Exceptions;
using AuthService.Application.Validators;
using AuthService.Domain.Constants;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AuthService.Application.DTOs.Email;
using AuthService.Application.Extensions;

namespace AuthService.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPasswordHashService passwordHashService,
    IJwtTokenService jwtTokenService,
    ICloudinaryService cloudinaryService,
    IEmailService emailService,
    IConfiguration configuration,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await userRepository.ExistsByEmailAsync(registerDto.Email))
        {
            logger.LogRegistrationWithExistingEmail();
            throw new BusinessException(ErrorCodes.EMAIL_ALREADY_EXISTS, "Email already exists");
        }

        if (await userRepository.ExistsByUsernameAsync(registerDto.Username))
        {
            logger.LogRegistrationWithExistingUsername();
            throw new BusinessException(ErrorCodes.USERNAME_ALREADY_EXISTS, "Username already exists");
        }

        string profilePicturePath;
        if (registerDto.ProfilePicture != null && registerDto.ProfilePicture.Size > 0)
        {
            var (isValid, errorMessage) = FileValidator.ValidateImage(registerDto.ProfilePicture);
            if (!isValid) throw new BusinessException(ErrorCodes.INVALID_FILE_FORMAT, errorMessage!);

            var fileName = FileValidator.GenerateSecureFileName(registerDto.ProfilePicture.FileName);
            profilePicturePath = await _cloudinaryService.UploadImageAsync(registerDto.ProfilePicture, fileName);
        }
        else
        {
            profilePicturePath = _cloudinaryService.GetDefaultAvatarUrl();
        }

        var emailVerificationToken = TokenGenerator.GenerateEmailVerificationToken();
        var userId = UuidGenerator.GenerateUserId();

        var defaultRole = await roleRepository.GetByNameAsync(RoleConstants.CLIENT_ROLE);
        if (defaultRole == null) throw new InvalidOperationException("Default role not found");

        var user = new User
        {
            Id = userId,
            UserName = registerDto.UserName,      // Corregido
            UserSurname = registerDto.UserSurname,  // Corregido
            Username = registerDto.Username,
            Email = registerDto.Email.ToLowerInvariant(),
            Password = passwordHashService.HashPassword(registerDto.Password),
            UserStatus = "INACTIVE",              // Empieza inactivo hasta verificar email
            UserCreatedAt = DateTime.UtcNow,
            UserProfile = new UserProfile
            {
                Id = UuidGenerator.GenerateUserId(),
                UserId = userId,
                ProfilePictureUrl = profilePicturePath,
                Phone = registerDto.Phone
            },
            UserEmail = new UserEmail
            {
                Id = UuidGenerator.GenerateUserId(),
                UserId = userId,
                EmailVerified = false,
                EmailVerificationToken = emailVerificationToken,
                EmailVerificationTokenExpiration = DateTime.UtcNow.AddHours(24)
            },
            UserRoles = [new UserRole { Id = UuidGenerator.GenerateUserId(), UserId = userId, RoleId = defaultRole.Id }]
        };

        var createdUser = await userRepository.CreateAsync(user);

        _ = Task.Run(async () =>
        {
            try { await emailService.SendEmailVerificationAsync(createdUser.Email, createdUser.Username, emailVerificationToken); }
            catch (Exception ex) { logger.LogError(ex, "Failed to send verification email"); }
        });

        return new RegisterResponseDto
        {
            Success = true,
            User = MapToUserResponseDto(createdUser),
            Message = "Usuario registrado exitosamente. Verifica tu email.",
            EmailVerificationRequired = true
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        User? user = loginDto.EmailOrUsername.Contains('@')
            ? await userRepository.GetByEmailAsync(loginDto.EmailOrUsername.ToLowerInvariant())
            : await userRepository.GetByUsernameAsync(loginDto.EmailOrUsername);

        if (user == null || !passwordHashService.VerifyPassword(loginDto.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        if (user.UserStatus != "ACTIVE")
            throw new UnauthorizedAccessException("User account is not active. Please verify your email.");

        var token = jwtTokenService.GenerateToken(user);
        var expiryMinutes = int.Parse(configuration["JwtSettings:ExpiryInMinutes"] ?? "30");

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login exitoso",
            Token = token,
            UserDetails = MapToUserDetailsDto(user),
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
    }

    private UserResponseDto MapToUserResponseDto(User user)
    {
        var userRole = user.UserRoles.FirstOrDefault()?.Role?.Name ?? RoleConstants.CLIENT_ROLE;
        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,       // Corregido
            UserSurname = user.UserSurname,   // Corregido
            Username = user.Username,
            Email = user.Email,
            Role = userRole,
            UserStatus = user.UserStatus,     // Corregido
            BranchId = user.BranchId,
            IsEmailVerified = user.UserEmail?.EmailVerified ?? false,
            UserCreatedAt = user.UserCreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    private UserDetailsDto MapToUserDetailsDto(User user)
    {
        return new UserDetailsDto
        {
            Id = user.Id,
            Username = user.Username,
            ProfilePicture = _cloudinaryService.GetFullImageUrl(user.UserProfile?.ProfilePictureUrl ?? string.Empty),
            Role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? RoleConstants.CLIENT_ROLE
        };
    }

    public async Task<EmailResponseDto> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
    {
        var user = await userRepository.GetByEmailVerificationTokenAsync(verifyEmailDto.Token);
        if (user == null || user.UserEmail == null)
            return new EmailResponseDto { Success = false, Message = "Token inválido" };

        user.UserEmail.EmailVerified = true;
        user.UserStatus = "ACTIVE"; // Activamos al usuario
        user.UserEmail.EmailVerificationToken = null;

        await userRepository.UpdateAsync(user);
        return new EmailResponseDto { Success = true, Message = "Email verificado" };
    }

    public async Task<EmailResponseDto> ResendVerificationEmailAsync(ResendVerificationDto resendDto)
    {
        var user = await userRepository.GetByEmailAsync(resendDto.Email);
        if (user == null) return new EmailResponseDto { Success = false, Message = "Usuario no encontrado" };
        if (user.UserEmail?.EmailVerified == true) return new EmailResponseDto { Success = false, Message = "El correo ya ha sido verificado" };

        var newToken = TokenGenerator.GenerateEmailVerificationToken();
        user.UserEmail!.EmailVerificationToken = newToken;
        user.UserEmail.EmailVerificationTokenExpiration = DateTime.UtcNow.AddHours(24);

        await userRepository.UpdateAsync(user);
        await emailService.SendEmailVerificationAsync(user.Email, user.Username, newToken);

        return new EmailResponseDto { Success = true, Message = "Correo de verificación reenviado" };
    }

    public async Task<EmailResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await userRepository.GetByEmailAsync(forgotPasswordDto.Email);
        if (user == null) return new EmailResponseDto { Success = true, Message = "Si el correo existe, se enviarán instrucciones" };

        var resetToken = TokenGenerator.GeneratePasswordResetToken();
        if (user.UserPasswordReset == null)
        {
            user.UserPasswordReset = new UserPasswordReset { Id = UuidGenerator.GenerateUserId(), UserId = user.Id };
        }

        user.UserPasswordReset.PasswordResetToken = resetToken;
        user.UserPasswordReset.PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(1);

        await userRepository.UpdateAsync(user);
        await emailService.SendPasswordResetAsync(user.Email, user.Username, resetToken);

        return new EmailResponseDto { Success = true, Message = "Correo de recuperación enviado" };
    }

    public async Task<EmailResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await userRepository.GetByPasswordResetTokenAsync(resetPasswordDto.Token);

        // Agregamos el ? después de UserPasswordReset para que sea seguro
        if (user == null || user.UserPasswordReset?.PasswordResetTokenExpiration < DateTime.UtcNow)
            return new EmailResponseDto { Success = false, Message = "Token inválido o expirado" };

        user.Password = passwordHashService.HashPassword(resetPasswordDto.NewPassword);

        // Aquí también usamos el ? por seguridad
        if (user.UserPasswordReset != null)
        {
            user.UserPasswordReset.PasswordResetToken = null;
        }

        await userRepository.UpdateAsync(user);
        return new EmailResponseDto { Success = true, Message = "Contraseña restablecida correctamente" };
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(string userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        return user == null ? null : MapToUserResponseDto(user);
    }
}