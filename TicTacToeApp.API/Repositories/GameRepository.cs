using TicTacToeApp.API.Entity;
using TicTacToeApp.API.Interfaces;

namespace TicTacToeApp.API.Repositories;

public class GameRepository : IGameRepository
{
    public List<Game> Games { get; set; } = new ()
    {
        new Game(),
        new Game(),
        new Game()
    };

    public Game FindGameByGuid(Guid id)
    {
        var game = Games.FirstOrDefault(g => g.Id == id);
        return game ?? throw new Exception("Нет такой игры!");
    }

    public List<Game> GetGames()
    {
        return Games.ToList();
    }
}