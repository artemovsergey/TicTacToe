using Microsoft.EntityFrameworkCore;
using TicTacToeApp.API.Data;
using TicTacToeApp.API.Entity;
using TicTacToeApp.API.Interfaces;

namespace TicTacToeApp.API.Repositories;

public sealed class GameRepository(TicTacToeContext db, ILogger<GameRepository> log) : IGameRepository
{
    public Game FindGameByGuid(Guid id)
    {
        var game = db.Games.FirstOrDefault(g => g.Id == id);
        return game ?? throw new Exception("Нет такой игры!");
    }

    public List<Game> GetGames()
    {
        return db.Games.AsNoTracking().ToList();
    }

    public Game CreateGame(int size)
    {
        if (size < 3) throw new ArgumentException("Поле для игры должно быть минимум 3 на 3");

        var game = new Game()
        {
            Board = CreateEmptyBoard(size)
        };
        db.Games.Add(game);
        db.SaveChanges();
        log.LogInformation($"Создана игра {game.Id}");
        return game;
    }

    public bool UpdateGame(Game game)
    {
        db.Games.Update(game);
        return db.SaveChanges() > 0;
    }

    private string?[][] CreateEmptyBoard(int size)
    {
        var board = new string?[size][];
        for (int i = 0; i < size; i++)
        {
            board[i] = new string?[size];
            for (int j = 0; j < size; j++)
            {
                board[i][j] = null;
            }
        }

        return board;
    }
}