using TicTacToeApp.API.Entity.Enums;

namespace TicTacToeApp.API.Entity;

public record Move(Player p, int x, int y);