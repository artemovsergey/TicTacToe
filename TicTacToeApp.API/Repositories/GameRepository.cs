using Microsoft.EntityFrameworkCore;
using TicTacToeApp.API.Data;
using TicTacToeApp.API.Entity;
using TicTacToeApp.API.Interfaces;

namespace TicTacToeApp.API.Repositories;

public sealed class GameAsyncRepository(TicTacToeContext db, ILogger<GameAsyncRepository> log) : IGameAsyncRepository
{
    public async Task<Game> FindGameByGuidAsync(Guid id, CancellationToken ct)
    {
        var game = await db.Games.FirstOrDefaultAsync(g => g.Id == id);
        return game ?? throw new Exception("Нет такой игры!");
    }

    public async Task<IEnumerable<Game>> GetGamesAsync(CancellationToken ct)
    {
        return await db.Games.AsNoTracking().ToListAsync();
    }

    public async Task<Game> CreateGameAsync(int size, CancellationToken ct)
    {
        if (size < 3) throw new ArgumentException("Поле для игры должно быть минимум 3 на 3");

        var game = new Game()
        {
            Board = CreateEmptyBoard(size)
        };
        db.Games.Add(game);
        await db.SaveChangesAsync();
        log.LogInformation($"Создана игра {game.Id}");
        return game;
    }

    public async Task<bool> UpdateGameAsync(Game game, CancellationToken ct)
    {
        db.Games.Update(game);
        return await db.SaveChangesAsync() > 0;
    }

    public string?[][] CreateEmptyBoard(int size)
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