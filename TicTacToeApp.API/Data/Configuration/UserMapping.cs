using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicTacToeApp.API.Data.Converters;
using TicTacToeApp.API.Entity;

namespace TicTacToeApp.API.Data.Configuration;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    { 
        builder.ToTable("Users").HasKey(u => u.Id);

        builder.Property(u => u.Name).HasColumnType("varchar(100)");
        // builder.Property(u => u.Age).HasColumnType("int");
        // builder.Property(u => u.Age2).HasColumnType("int");
        // builder.Property(u => u.Age3).HasColumnType("int");


        //
        // builder.Property(g => g.CreatedAt)
        //     .HasColumnType("timestamp with time zone");
        //     //.HasConversion(new DateTimeToCharConverter());
        //
        // builder.Property(g => g.Id).HasColumnType("uuid");
        // builder.Property(g => g.Status).HasConversion<string>();
        // builder.Property(g => g.Result).HasConversion<string>();
        // builder.Property(g => g.CurrentMove).HasConversion<string>();
        // builder.Property(g => g.Board).HasConversion(
        //     v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
        //     v => JsonSerializer.Deserialize<string?[][]>(v, (JsonSerializerOptions?)null)!);
        //
        // builder.HasOne(g => g.User)
        //        .WithMany(user => user.Games)
        //        .HasPrincipalKey(g =>g.Id)
        //        .HasForeignKey(u => u.UserId);

        // Seed
        // builder.HasData( new Game());
    }
}