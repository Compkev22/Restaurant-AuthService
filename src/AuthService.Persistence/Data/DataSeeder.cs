using AuthService.Domain.Entities;
using AuthService.Application.Services;
using AuthService.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using AuthService.Application.Interfaces; // <-- SUPER IMPORTANTE

namespace AuthService.Persistence.Data;

public static class DataSeeder
{
    // AHORA RECIBIMOS LOS 2 ARGUMENTOS AQUÍ
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHashService passwordHashService)
    {
        // 1. Sembrar Roles
        if (!context.Roles.Any())
        {
            var roles = new List<Role>
            {
                new() { Id = UuidGenerator.GenerateRoleId(), Name = RoleConstants.PLATFORM_ADMIN_ROLE },
                new() { Id = UuidGenerator.GenerateRoleId(), Name = RoleConstants.BRANCH_ADMIN_ROLE },
                new() { Id = UuidGenerator.GenerateRoleId(), Name = RoleConstants.EMPLOYEE_ROLE },
                new() { Id = UuidGenerator.GenerateRoleId(), Name = RoleConstants.CLIENT_ROLE }
            };
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // 2. Sembrar Admin Maestro
        if (!await context.Users.AnyAsync())
        {
            var platformAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == RoleConstants.PLATFORM_ADMIN_ROLE);
            
            if (platformAdminRole != null)
            {
                var userId = UuidGenerator.GenerateUserId();
                var adminUser = new User
                {
                    Id = userId,
                    UserName = "Platform",
                    UserSurname = "Admin",
                    Username = "sysadmin",
                    Email = "admin@restaurante.local",
                    // USAMOS EL SERVICIO DE HASH PARA LA CONTRASEÑA
                    Password = passwordHashService.HashPassword("12345678"), 
                    UserStatus = "ACTIVE",
                    UserCreatedAt = DateTime.UtcNow,
                    UserProfile = new UserProfile { Id = UuidGenerator.GenerateUserId(), UserId = userId },
                    UserEmail = new UserEmail { Id = UuidGenerator.GenerateUserId(), UserId = userId, EmailVerified = true },
                    UserRoles = [ new UserRole { Id = UuidGenerator.GenerateUserId(), UserId = userId, RoleId = platformAdminRole.Id } ]
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}