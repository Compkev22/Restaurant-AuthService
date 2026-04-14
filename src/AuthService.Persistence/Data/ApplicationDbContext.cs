using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserEmail> UserEmails { get; set; }
    public DbSet<UserPasswordReset> UserPasswordResets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Aplicar Snake Case general
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName)) entity.SetTableName(ToSnakeCase(tableName));

            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (!string.IsNullOrEmpty(columnName)) property.SetColumnName(ToSnakeCase(columnName));
            }
        }

        // 2. CONFIGURACIÓN ESPECÍFICA PARA COMPATIBILIDAD CON NODE.JS (Mongoose)
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();

            // FORZAR NOMBRES EXACTOS PARA EL FRONTEND/NODE
            entity.Property(e => e.UserName).HasColumnName("UserName"); 
            entity.Property(e => e.UserSurname).HasColumnName("UserSurname");
            entity.Property(e => e.UserStatus).HasColumnName("UserStatus");
            entity.Property(e => e.UserCreatedAt).HasColumnName("UserCreatedAt");

            // Relaciones
            entity.HasOne(e => e.UserProfile).WithOne(p => p.User).HasForeignKey<UserProfile>(p => p.UserId);
            entity.HasMany(e => e.UserRoles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId);
            entity.HasOne(e => e.UserEmail).WithOne(ue => ue.User).HasForeignKey<UserEmail>(ue => ue.UserId);
            entity.HasOne(e => e.UserPasswordReset).WithOne(upr => upr.User).HasForeignKey<UserPasswordReset>(upr => upr.UserId);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
        });
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString().ToLower() : x.ToString().ToLower()));
    }
}