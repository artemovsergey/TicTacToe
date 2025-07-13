using Microsoft.EntityFrameworkCore;
using TicTacToeApp.API.Data;
using TicTacToeApp.API.Entity;
using TicTacToeApp.API.Exceptions;
using TicTacToeApp.API.Interfaces;
using TicTacToeApp.API.Services;

namespace TicTacToeApp.API.Repositories;

public sealed class GameAsyncRepository(TicTacToeContext db, ILogger<GameAsyncRepository> log) : IGameAsyncRepository
{
    public async Task<Game> FindGameByGuidAsync(Guid id, CancellationToken ct)
    {
        const string errorMessage = "Нет такой игры";
        var game = await db.Games.FirstOrDefaultAsync(g => g.Id == id, ct);
        log.LogError(errorMessage);
        return game ?? throw new NotFoundException(errorMessage);
    }

    public async Task<IEnumerable<Game>> GetGamesAsync(CancellationToken ct)
    {
        log.LogInformation("Все игры получены!");
        return await db.Games.AsNoTracking().ToListAsync(ct);
    }

    public async Task<Game> CreateGameAsync(int size, CancellationToken ct)
    {
        if (size < 3) throw new ArgumentException("Поле для игры должно быть минимум 3 на 3");

        var game = new Game()
        {
            Board = GameService.CreateEmptyBoard(size)
        };
        await db.Games.AddAsync(game, ct);
        await db.SaveChangesAsync(ct);
        log.LogInformation($"Создана игра {game.Id}");
        return game;
    }

    public async Task<bool> UpdateGameAsync(Game game, CancellationToken ct)
    {
        db.Games.Update(game);
        log.LogInformation($"Создана игра {game.Id}");
        return await db.SaveChangesAsync(ct) > 0;
    }
    
}