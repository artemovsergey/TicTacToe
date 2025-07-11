using TicTacToeApp.API.Entity;

namespace TicTacToeApp.API.Interfaces;

public interface IGameAsyncRepository
{
    Task<Game> FindGameByGuidAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Game>> GetGamesAsync(CancellationToken ct);
    Task<Game> CreateGameAsync(int size, CancellationToken ct);
    Task<bool> UpdateGameAsync(Game game, CancellationToken ct);
}