using AuthService.Domain.Entities;
using AuthService.Application.Services;
using AuthService.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // 1. Sembrar los 4 roles del restaurante si la tabla está vacía
        if (!context.Roles.Any())
        {
            var roles = new List<Role>
            {
                new() { 
                    Id = UuidGenerator.GenerateRoleId(), 
                    Name = RoleConstants.PLATFORM_ADMIN_ROLE 
                },
                new() { 
                    Id = UuidGenerator.GenerateRoleId(), 
                    Name = RoleConstants.BRANCH_ADMIN_ROLE 
                },
                new() { 
                    Id = UuidGenerator.GenerateRoleId(), 
                    Name = RoleConstants.EMPLOYEE_ROLE 
                },
                new() { 
                    Id = UuidGenerator.GenerateRoleId(), 
                    Name = RoleConstants.CLIENT_ROLE 
                }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // 2. Crear al "Platform Admin" maestro si no existen usuarios
        if (!await context.Users.AnyAsync())
        {
            var platformAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == RoleConstants.PLATFORM_ADMIN_ROLE);
            
            if (platformAdminRole != null)
            {
                var userId = UuidGenerator.GenerateUserId();
                var profileId = UuidGenerator.GenerateUserId();
                var emailId = UuidGenerator.GenerateUserId();
                var userRoleId = UuidGenerator.GenerateUserId();

                var adminUser = new User
                {
                    Id = userId,
                    Name = "Platform",
                    Surname = "Admin",
                    Username = "sysadmin",
                    Email = "admin@restaurante.local",
                    Password = "12345678", // Contraseña temporal
                    Status = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserProfile = new UserProfile
                    {
                        Id = profileId,
                        UserId = userId
                    },
                    UserEmail = new UserEmail
                    {
                        Id = emailId,
                        UserId = userId,
                        EmailVerified = true
                    },
                    UserRoles =
                    [
                        new UserRole
                        {
                            Id = userRoleId,
                            UserId = userId,
                            RoleId = platformAdminRole.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        }
                    ]
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}