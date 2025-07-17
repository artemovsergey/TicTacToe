using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicTacToeApp.API.Data.Converters;
using TicTacToeApp.API.Entity;

namespace TicTacToeApp.API.Data.Configuration;

public class GameMapping : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games").HasKey(g => g.Id);
        builder.Property(g => g.CreatedAt)
                .HasColumnType("timestamp with time zone");
        //.HasConversion(new DateTimeToCharConverter());

        builder.Property(g => g.Status).HasConversion<string>();
        builder.Property(g => g.Result).HasConversion<string>();
        builder.Property(g => g.CurrentMove).HasConversion<string>();
        builder.Property(g => g.Board).HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<string?[][]>(v, (JsonSerializerOptions?)null)!);

        builder.Property(g => g.Board)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<string?[][]>(v, (JsonSerializerOptions?)null)!)
            .Metadata.SetValueComparer(
                new ValueComparer<string?[][]>(
                    (c1, c2) => c1!.SequenceEqual(c2!), // Сравниваем массивы поэлементно
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Вычисляем хеш-код
                    c => c.Select(x => x.ToArray()).ToArray())); // Создаем глубокую копию для snapshot
        
        
        // builder.HasOne(g => g.User)
        //        .WithMany(user => user.Games)
        //        .HasPrincipalKey(g =>g.Id)
        //        .HasForeignKey(u => u.UserId);

        // Seed
         builder.HasData(
             new Game
             {
                 Id = new Guid("b8aa36a5-9094-4dbd-80a5-444fcb92d3a4"), // Укажите явный Id (если есть)
                 Board = new string?[][] 
                 {
                     new string?[] { "X", "O", null },
                     new string?[] { null, "X", "O" },
                     new string?[] { "O", null, "X" }
                 },
                 CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                 
             }
         );
    }
}