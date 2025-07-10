using TicTacToeApp.API.Entity;

namespace TicTacToeApp.API.Interfaces;

public interface IGameRepository
{
    Game FindGameByGuid(Guid id);
    List<Game> GetGames();
    Game CreateGame(int size);
    bool UpdateGame(Game game);
}