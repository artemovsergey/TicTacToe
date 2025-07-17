using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TicTacToeApp.API.Entity.Enums;

namespace TicTacToeApp.API.Entity;

public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public StatusGame Status { get; set; } = StatusGame.Active;
    
    // [NotMapped]
    public string?[][] Board { get; set; } = null!;
    public ResultGame Result { get; set; } = ResultGame.None;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Player CurrentMove { get; set; } = Player.X;
    public uint CurrentStep { get; set; } = 0;
    
    // public User User { get; set; } = default!;
    // public int UserId { get; set; }
}

