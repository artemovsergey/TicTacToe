using TicTacToeApp.API.Data.Configuration;
using TicTacToeApp.API.Entity;

namespace TicTacToeApp.API.Data;

using Microsoft.EntityFrameworkCore;

public class TicTacToeContext : DbContext
{
    public TicTacToeContext(DbContextOptions<TicTacToeContext> opt) : base(opt)
    {
        // Database.Migrate();
    }

    public DbSet<Game> Games { get; set; }
    // public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GameMapping());
        // modelBuilder.ApplyConfiguration(new UserMapping());
        
        base.OnModelCreating(modelBuilder);
    }
}