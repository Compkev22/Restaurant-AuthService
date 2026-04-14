using System;
using System.Collections.Generic;

namespace AuthService.Domain.Entities;

public class User
{
    // Propiedades básicas (Las que antes buscaba en BaseEntity)
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Propiedades de compatibilidad con Node.js (Mongoose)
    public string UserName { get; set; } = string.Empty;
    public string UserSurname { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    // Cambiamos de bool a string para usar 'ACTIVE' / 'INACTIVE'
    public string UserStatus { get; set; } = "ACTIVE";
    
    public string? BranchId { get; set; }

    // Campos extra de auditoría
    public DateTime UserCreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    // Relaciones (Navegación)
    public virtual UserProfile? UserProfile { get; set; }
    public virtual UserEmail? UserEmail { get; set; }
    public virtual UserPasswordReset? UserPasswordReset { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}