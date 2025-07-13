using System.Text.Json;
using TicTacToeApp.API.Entity;

namespace TicTacToeApp.API.Data;

using Microsoft.EntityFrameworkCore;

public sealed class TicTacToeContext : DbContext
{
    public TicTacToeContext(DbContextOptions<TicTacToeContext> opt) : base(opt)
    {
        Database.EnsureDeleted();
        Database.Migrate();
    }

    public DbSet<Game> Games { get; set; }
    // public DbSet<Move> Moves { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Move>(entity =>
        // {
        //     entity.HasNoKey();
        // });
        
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(g => g.Id);

            entity.Property(g => g.Status)
                .HasConversion<string>();

            entity.Property(g => g.Result)
                .HasConversion<string>();

            entity.Property(g => g.CurrentMove)
                .HasConversion<string>();

            // Для хранения массива массивов (Board) используем JSON сериализацию
            entity.Property(g => g.Board)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<string?[][]>(v, (JsonSerializerOptions?)null)!);
        });
        
    }
}