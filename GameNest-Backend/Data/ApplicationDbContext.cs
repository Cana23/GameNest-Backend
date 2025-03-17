namespace GameNest_Backend.Data;
using GameNest_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<Follower> Followers { get; set; }
    public DbSet<Like> Likes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Usuario)  // Usar la propiedad de navegación User
            .WithMany()  // No necesitas una colección en User para los comentarios
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);  // Evita eliminación en cascada

        modelBuilder.Entity<Follower>()
            .HasOne(f => f.UsuarioSeguidor)
            .WithMany()
            .HasForeignKey(f => f.UsuarioSeguidorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follower>()
            .HasOne(f => f.UsuarioSeguido)
            .WithMany()
            .HasForeignKey(f => f.UsuarioSeguidoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Añadir índice único para evitar que un usuario siga a otro más de una vez
        modelBuilder.Entity<Follower>()
            .HasIndex(f => new { f.UsuarioSeguidorId, f.UsuarioSeguidoId })
            .IsUnique();

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Usuario)
            .WithMany()
            .HasForeignKey(f => f.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<Like>()
        //    .HasOne(l => l.PublicacionId)
        //    .WithMany()
        //    .HasForeignKey(f => f.PublicacionId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<Like>()
        //.HasIndex(f => new { f.UsuarioId, f.PublicacionId })
        //.IsUnique();
    }


}
