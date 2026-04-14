using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Constants;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services;

public class UserManagementService(IUserRepository users, IRoleRepository roles, ICloudinaryService cloudinary) : IUserManagementService
{
    public async Task<UserResponseDto> UpdateUserRoleAsync(string userId, string roleName)
    {
        // Normalize
        roleName = roleName?.Trim().ToUpperInvariant() ?? string.Empty;

        // Validate inputs
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Invalid userId", nameof(userId));

        if (!RoleConstants.AllowedRoles.Contains(roleName))
            throw new InvalidOperationException($"Role not allowed. Use {RoleConstants.PLATFORM_ADMIN_ROLE}, {RoleConstants.BRANCH_ADMIN_ROLE}, {RoleConstants.EMPLOYEE_ROLE} or {RoleConstants.CLIENT_ROLE}");

        // Load user with roles
        var user = await users.GetByIdAsync(userId);

        // If demoting an admin, prevent removing last platform admin
        var isUserAdmin = user.UserRoles.Any(r => r.Role.Name == RoleConstants.PLATFORM_ADMIN_ROLE);
        if (isUserAdmin && roleName != RoleConstants.PLATFORM_ADMIN_ROLE)
        {
            var adminCount = await roles.CountUsersInRoleAsync(RoleConstants.PLATFORM_ADMIN_ROLE);

            if (adminCount <= 1)
            {
                throw new InvalidOperationException("Cannot remove the last administrator");
            }
        }

        // Find role entity
        var role = await roles.GetByNameAsync(roleName)
                       ?? throw new InvalidOperationException($"Role {roleName} not found");

        // Update role using repository method
        await users.UpdateUserRoleAsync(userId, role.Id);

        // Reload user with updated roles
        user = await users.GetByIdAsync(userId);

        // Map to response
        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            UserSurname = user.UserSurname,
            Username = user.Username,
            Email = user.Email,
            ProfilePicture = cloudinary.GetFullImageUrl(user.UserProfile?.ProfilePictureUrl ?? string.Empty),
            Phone = user.UserProfile?.Phone ?? string.Empty,
            Role = role.Name,
            UserStatus = user.UserStatus,
            IsEmailVerified = user.UserEmail?.EmailVerified ?? false,
            UserCreatedAt = user.UserCreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<IReadOnlyList<string>> GetUserRolesAsync(string userId)
    {
        var roleNames = await roles.GetUserRoleNamesAsync(userId);
        return roleNames;
    }

    public async Task<IReadOnlyList<UserResponseDto>> GetUsersByRoleAsync(string roleName)
    {
        roleName = roleName?.Trim().ToUpperInvariant() ?? string.Empty;
        var usersInRole = await roles.GetUsersByRoleAsync(roleName);

        return usersInRole.Select(u => new UserResponseDto
        {
            Id = u.Id,
            UserName = u.UserName,       // Antes decía u.Name
            UserSurname = u.UserSurname,   // Antes decía u.Surname
            Username = u.Username,
            Email = u.Email,
            ProfilePicture = cloudinary.GetFullImageUrl(u.UserProfile?.ProfilePictureUrl ?? string.Empty),
            Phone = u.UserProfile?.Phone ?? string.Empty,
            Role = roleName,
            UserStatus = u.UserStatus,     // Antes decía u.Status
            IsEmailVerified = u.UserEmail?.EmailVerified ?? false,
            UserCreatedAt = u.UserCreatedAt, // Antes decía u.CreatedAt
            UpdatedAt = u.UpdatedAt
        }).ToList();
    }
}