using TicTacToeApp.API.Entity.Enums;

namespace TicTacToeApp.API.Entity;

public sealed class Game
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public StatusGame Status { get; set; } = StatusGame.Active;

    public int?[][] Board { get; set; } =
    {
        new int?[] { null, null, null },
        new int?[] { null, null, null },
        new int?[] { null, null, null }
    };

    public ResultGame Result { get; set; } = ResultGame.None;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Player CurrentMove { get; set; } = Player.X;
    public uint CurrentStep { get; set; } = 0;
}