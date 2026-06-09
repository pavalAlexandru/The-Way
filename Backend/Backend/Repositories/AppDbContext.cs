using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class AppDbContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<GameConfig> GameConfigs { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<Guess> Guesses { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>().HasData(
            new Player { Id = 1, Username = "Andu" }
        );
        modelBuilder.Entity<GameConfig>().HasData(
            new GameConfig { Id = 1, SafePath = "2,3,1,4", Obstacles = "1-1,4-2,2-3,3-4,1-4" }
        );
    }
}
