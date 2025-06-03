using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PixelForge.Areas.Identity.Data;
using PixelForge.Models;
using System.Reflection.Emit;

namespace PixelForge.Areas.Identity.Data;

public class UserDbContext : IdentityDbContext<PixelForgeUser>
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    public DbSet<PixelForgeUser> Users { get; set; }
    public DbSet<UserGame> UserGames { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ReviewVote> ReviewVotes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserGame>()
            .HasKey(ug => new { ug.UserId, ug.GameId });

        builder.Entity<UserGame>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.UserGames)
            .HasForeignKey(ug => ug.UserId);

        builder.Entity<UserGame>()
            .HasOne(ug => ug.Game)
            .WithMany(g => g.UserGames)
            .HasForeignKey(ug => ug.GameId);

        builder.Entity<Game>()
            .HasOne(g => g.Publisher)
            .WithMany()
            .HasForeignKey(g => g.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasIndex(r => new { r.UserId, r.GameId })
            .IsUnique();

        builder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Review>()
            .HasOne(r => r.Game)
            .WithMany(g => g.Reviews)
            .HasForeignKey(r => r.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ReviewVote>()
            .HasKey(rv => new { rv.UserId, rv.ReviewId });

        builder.Entity<ReviewVote>()
            .HasOne(rv => rv.User)
            .WithMany()
            .HasForeignKey(rv => rv.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ReviewVote>()
            .HasOne(rv => rv.Review)
            .WithMany(r => r.Votes)
            .HasForeignKey(rv => rv.ReviewId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
