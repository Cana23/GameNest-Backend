using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GameNest_Backend.Models;
using Microsoft.AspNetCore.Identity;
using System;

public class ApplicationDbContext : IdentityDbContext<
    User,
    IdentityRole<Guid>, // Tipo de Role
    Guid, // Tipo de clave primaria (TKey)
    IdentityUserClaim<Guid>, // Tipo de UserClaim
    IdentityUserRole<Guid>, // Tipo de UserRole
    IdentityUserLogin<Guid>, // Tipo de UserLogin
    IdentityRoleClaim<Guid>, // Tipo de RoleClaim
    IdentityUserToken<Guid> // Tipo de UserToken
>
{
    public DbSet<Publication> Publications { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Follower> Followers { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<RevokedToken> RevokedTokens { get; set; } 


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Comment
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Publicacion)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PublicacionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de Follower
        modelBuilder.Entity<Follower>()
            .HasOne(f => f.UsuarioSeguidor)
            .WithMany(u => u.Siguiendo)
            .HasForeignKey(f => f.UsuarioSeguidorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follower>()
            .HasOne(f => f.UsuarioSeguido)
            .WithMany(u => u.Seguidores)
            .HasForeignKey(f => f.UsuarioSeguidoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follower>()
            .HasIndex(f => new { f.UsuarioSeguidorId, f.UsuarioSeguidoId })
            .IsUnique();

        // Configuración de Like
        modelBuilder.Entity<Like>()
            .HasOne(l => l.Usuario)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Publicacion)
            .WithMany(p => p.Likes)
            .HasForeignKey(l => l.PublicacionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice único para evitar likes duplicados
        modelBuilder.Entity<Like>()
            .HasIndex(l => new { l.UsuarioId, l.PublicacionId })
            .IsUnique();

        // Configuración de Publication
        modelBuilder.Entity<Publication>()
            .HasOne(p => p.User)
            .WithMany(u => u.Publications)
            .HasForeignKey(p => p.UserId);
    }
}