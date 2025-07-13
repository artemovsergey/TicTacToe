using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using TicTacToeApp.API.Dtos;
using TicTacToeApp.API.Entity;
using TicTacToeApp.API.Entity.Enums;
using TicTacToeApp.API.Interfaces;
using TicTacToeApp.API.Response;
using TicTacToeApp.API.Services;
using TicTacToeApp.API.Validations;

namespace TicTacToeApp.API.Endpoints;

public static class GameEndpoints
{
    public static WebApplication UseGameEndpoints(
        this WebApplication app
    )
    {
        int TICTACTOE_BOARD_SIZE = int.Parse(Environment.GetEnvironmentVariable("TICTACTOE_BOARD_SIZE")!);

        int TICTACTOE_LINE_TO_WIN = int.Parse(Environment.GetEnvironmentVariable("TICTACTOE_LINE_TO_WIN")!);

        int TICTACTOE_CHANCE =
            int.Parse(Environment.GetEnvironmentVariable("TICTACTOE_LINE_TO_WIN")!); // процент вероятности замены хода
        int TICTACTOE_NUMBER_STEP =
            int.Parse(Environment.GetEnvironmentVariable("TICTACTOE_LINE_TO_WIN")!); // на каком ходу

        app.MapGet("/api/games",
                async (IGameAsyncRepository repo, CancellationToken ct) =>
                {
                    return Results.Ok(await repo.GetGamesAsync(ct));
                })
            .WithTags("TicTacToeApp.API")
            .WithName("GetAllGames")
            .WithSummary("Список доступных игр")
            .WithDescription("Возвращает список объектов Game")
            .Produces<List<Game>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        app.MapGet("/api/games/{Id:guid}",
                async (IGameAsyncRepository repo, Guid Id, CancellationToken ct) =>
                {
                    return Results.Ok(await repo.FindGameByGuidAsync(Id, ct));
                })
            .WithName("GetGameById")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Получение игры по Id";
                operation.Description = "Возвращает объект Game";
                return operation;
            })
            .Produces<Game>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        app.MapPost("/api/games/new", async (IGameAsyncRepository repo, GameOption? gameOption, CancellationToken ct) =>
            {
                
                if (gameOption.size < 3 || gameOption.line_to_win < 1 || gameOption.chance < 1 || gameOption.step < 1 )
                    return Results.Json<ErrorResponse>(new ErrorResponse(
                            statusCode: "400",
                            message: "Размерность должна быть больше от 3, условие победы, вероятность замены и шаг вероятности положительны"
                        ),
                        statusCode: StatusCodes.Status400BadRequest,
                        contentType: "application/json"
                    );

                if (gameOption.line_to_win > gameOption.size)
                    return Results.Json<ErrorResponse>(new ErrorResponse(
                            statusCode: "400",
                            message: "Количество одинаковых элементов должно быть меньше или равно размерности доски!"
                        ),
                        statusCode: StatusCodes.Status400BadRequest,
                        contentType: "application/json"
                    );
                
                
                TICTACTOE_BOARD_SIZE = gameOption!.size;
                TICTACTOE_LINE_TO_WIN = gameOption.line_to_win;
                TICTACTOE_CHANCE = gameOption.chance;
                TICTACTOE_NUMBER_STEP = gameOption.step;

                var game = await repo.CreateGameAsync(TICTACTOE_BOARD_SIZE, ct);

                return Results.CreatedAtRoute("GetGameById", game, value: game);
            })
            .WithTags("TicTacToeApp.API")
            .WithName("CreateGame")
            .WithSummary("Создание новой игры")
            .WithDescription("Возвращает объект игры Game")
            .Produces<Game>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        app.MapPost("api/games/{gameId:guid}/move",
                async (HttpResponse r,
                    [FromHeader(Name = "If-Match")] string? ifMatchHeader,
                    IGameAsyncRepository repo,
                    Guid gameId,
                    Move move,
                    ILogger<Program> log,
                    CancellationToken ct) =>
                {
                    var game = await repo.FindGameByGuidAsync(gameId, ct);

                    var currentETag = EtagService.GenerateETag(game);

                    if (ifMatchHeader != null && ifMatchHeader != currentETag)
                    {
                        log.LogError("Etag не совпадает");
                        return Results.Json(
                            new ErrorResponse(
                                statusCode: "412",
                                message: "Обновите состояние игры"
                            ),
                            statusCode: StatusCodes.Status412PreconditionFailed
                        );
                    }

                    if (game.Status != StatusGame.Active)
                    {
                        log.LogError("Игра завершена");
                        return Results.BadRequest(new ErrorResponse(
                                statusCode: "400",
                                message: $"Данная игра уже завершена! Итог: {game.Result}"
                            )
                        );
                    }

                    if (move.x < 0 || move.x > TICTACTOE_BOARD_SIZE - 1 || move.y < 0 ||
                        move.y > TICTACTOE_BOARD_SIZE - 1)
                    {
                        log.LogError("Координаты выходят за пределы поля");
                        return Results.BadRequest(new ErrorResponse(
                                statusCode: "400",
                                message: $"Неверные координаты доски. Разрешено: 0-{TICTACTOE_BOARD_SIZE - 1}"
                            )
                        );
                    }

                    if (game.CurrentMove != move.p)
                    {
                        log.LogError("Ход вне очереди");
                        return Results.BadRequest(new ErrorResponse(
                                statusCode: "400",
                                message: $"Не ваш ход! Сейчас ход: {game.CurrentMove}"
                            )
                        );
                    }

                    if (game.Board[move.x][move.y] != null)
                    {
                        log.LogError($"Ячейка ({move.x},{move.y}) занята");
                        return Results.Conflict(new ErrorResponse(
                                statusCode: "409",
                                message: $"Нельзя осуществить данный ход! Ячейка занята!"
                            )
                        );
                    }

                    game.Board[move.x][move.y] = (move.p.ToString());
                    game.CurrentStep += 1;

                    // Проверяем особое условие на каждый n ход с шансом m % замена выбора

                    bool maybeReplace = false;
                    if (game.CurrentStep > 0 && game.CurrentStep % TICTACTOE_NUMBER_STEP == 0)
                    {
                        var random = new Random();
                        double probability = TICTACTOE_CHANCE / 100.0; // %
                        maybeReplace = random.NextDouble() < probability; // true с вероятностью %

                        if (maybeReplace)
                        {
                            log.LogWarning(
                                $"Сработала вероятность {TICTACTOE_CHANCE}%. Текущий ход: {game.CurrentStep}. Выбор игрока заменен на противоположный!");
                            game.Board[move.x][move.y] = move.p == Player.X ? Player.O.ToString() : Player.X.ToString();
                        }
                    }

                    game.CurrentMove = move.p == Player.X ? Player.O : Player.X;

                    game.Result = GameService.CheckBoardN(game.Board, (move.p).ToString(), TICTACTOE_LINE_TO_WIN);

                    if (game.Result != ResultGame.None)
                    {
                        game.Status = StatusGame.Complete;
                    }

                    await repo.UpdateGameAsync(game, ct);

                    var response = new
                    {
                        GameId = gameId,
                        Board = game.Board,
                        Status = game.Status,
                        Result = game.Result,
                        DateTime = DateTime.UtcNow,
                        CurrentStep = game.CurrentStep,
                        CurrentMove = game.CurrentMove,
                        ReplaceMove = maybeReplace
                    };

                    r.Headers.ETag = EtagService.GenerateETag(game);
                    return Results.Ok(response);
                })
            .WithOpenApi(operation =>
            {
                operation.Summary = "Ход игрока X или O";
                operation.Description = "Возвращает состояние игры в виде объекта Game";
                return operation;
            })
            .Produces<Game>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status412PreconditionFailed);
        
        return app;
    }
}