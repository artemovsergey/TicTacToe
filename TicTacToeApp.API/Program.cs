using Newtonsoft.Json;
using TicTacToeApp.API.Entity;
using TicTacToeApp.API.Interfaces;
using TicTacToeApp.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IGameRepository, GameRepository>();

var app = builder.Build();
app.MapOpenApi();

app.MapGet("/api/games", (IGameRepository repo) =>
{
    return Results.Ok(repo.GetGames().Select(g => new{Id = g.Id}));
});

app.MapGet("/api/games/{Id}", (IGameRepository repo, Guid Id) =>
{
    return Results.Ok(repo.FindGameByGuid(Id));
});

app.MapPost("api/game/new", () =>
{
    var game = new Game();
    return Results.Ok(game);
});

app.MapPost("api/game/{game_id:guid}/move", (IGameRepository repo, Guid game_id, Move move) =>
{
    var game = repo.FindGameByGuid(game_id);
    game.Board[move.x][move.y] = (int)(move.p);
    game.CurrentStep += 1;

    var response = new
    {
        GameId = game_id,
        Board = game.Board,
        Status = game.Status,
        Result = game.Result,
        DateTime = DateTime.UtcNow,
        CurrentStep = game.CurrentStep
    };

    return Results.Ok(response);
});


app.Run();