using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
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
        this WebApplication app,
        CancellationToken cancellationToken
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
                return Results.Ok((await repo.GetGamesAsync(ct)).Select(g => new { Id = g.Id }));
            }).WithTags("Games")
            .WithName("GetAllGames")
            .WithSummary("Список доступных игр")
            .WithDescription("Возвращает список объектов GameDto")
            .Produces<List<Game>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        app.MapGet("/api/games/{Id}",
                async (IGameAsyncRepository repo, Guid Id, CancellationToken ct) =>
                {
                    return Results.Ok(await repo.FindGameByGuidAsync(Id, ct));
                }).WithName("GetGameById")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Получение игры по Id";
                operation.Description = "Возвращает объект Game";
                return operation;
            }).Produces<Game>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
        ;

        app.MapPost("/api/games/new", async (IGameAsyncRepository repo, CancellationToken ct) =>
            {
                if (TICTACTOE_LINE_TO_WIN > TICTACTOE_BOARD_SIZE)
                    return Results.BadRequest(
                        "Количество одинаковых элементов должно быть меньше или равно размерности доски!");

                var game = await repo.CreateGameAsync(TICTACTOE_BOARD_SIZE, ct);
                return Results.CreatedAtRoute("GetGameById", game);
            }).WithOpenApi(operation =>
            {
                operation.Summary = "Создание новой игры";
                operation.Description = "Возвращает объект Game";
                return operation;
            }).Produces<Game>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        app.MapPost("api/games/{game_id:guid}/move",
                async (HttpResponse r,
                    [FromHeader(Name = "If-Match")] string? ifMatchHeader,
                    IGameAsyncRepository repo,
                    MoveValidator moveValidator,
                    GameValidator gameValidator,
                    Guid game_id,
                    Move move,
                    CancellationToken ct) =>
                {
                    var game = await repo.FindGameByGuidAsync(game_id, ct);

                    var currentETag = EtagService.GenerateETag(game);
                    if (ifMatchHeader != null && ifMatchHeader != currentETag)
                    {
                        return Results.StatusCode(StatusCodes.Status412PreconditionFailed);
                    }


                    // Проверяем статус игры - активная она или завершена

                    if (game.Status != StatusGame.Active)
                    {
                        return Results.BadRequest($"Данная игра уже завершена! Итог: {game.Result}");
                    }

                    // Проверяем верные ли координаты

                    if (move.x < 0 || move.x > 2 || move.y < 0 || move.y > 2)
                    {
                        return Results.BadRequest("Неверные координаты доски. Разрешено: 0-2");
                    }

                    // Проверка очередности хода 

                    if (game.CurrentMove != move.p)
                    {
                        return Results.BadRequest($"Не ваш ход! Сейчас ход: {game.CurrentMove}");
                    }

                    // Проверяем ход игрока

                    if (game.Board[move.x][move.y] != null)
                    {
                        return Results.Conflict($"Нельзя осуществить данный ход! Ячейка занята!");
                    }

                    // Записываем ход игрока
                    game.Board[move.x][move.y] = (move.p.ToString());


                    // Наращиваем счетчик ходов в игре

                    game.CurrentStep += 1;

                    // Проверяем особое условие на каждый 2 ход с шансом 50% замены выбора
                    bool maybeReplace = false;
                    if (game.CurrentStep > 0 && game.CurrentStep % TICTACTOE_NUMBER_STEP == 0)
                    {
                        var random = new Random();
                        double probability = TICTACTOE_CHANCE / 100; // 50%
                        maybeReplace = random.NextDouble() < probability; // true с вероятностью 50%

                        Console.WriteLine($"Замена хода: {maybeReplace}");
                        if (maybeReplace)
                        {
                            Console.WriteLine(
                                $"Сработала вероятность 50%. Текущий ход: {game.CurrentStep}. Выбор игрока заменен на противоположный!");
                            game.Board[move.x][move.y] = move.p == Player.X ? Player.O.ToString() : Player.X.ToString();
                        }
                    }


                    // Обновление ожидаемого игрока 

                    game.CurrentMove = move.p == Player.X ? Player.O : Player.X;


                    // Проверяем состояние доски

                    game.Result = GameService.CheckBoardN(game.Board, (move.p).ToString(), TICTACTOE_LINE_TO_WIN);
                    if (game.Result != ResultGame.None)
                    {
                        game.Status = StatusGame.Complete;
                    }

                    await repo.UpdateGameAsync(game, ct);

                    // Формируем ответ 

                    var response = new
                    {
                        GameId = game_id,
                        Board = game.Board,
                        Status = game.Status,
                        Result = game.Result,
                        DateTime = DateTime.UtcNow,
                        CurrentStep = game.CurrentStep,
                        CurrentMove = game.CurrentMove,
                        ReplaceMove = maybeReplace
                    };

                    // Отправляем ответ c новым Еtag
                    r.Headers.ETag = EtagService.GenerateETag(game);
                    return Results.Ok(response);
                }).WithOpenApi(operation =>
            {
                operation.Summary = "Ход игрока X или O";
                operation.Description = "Возвращает состояние игры в виде объекта game";
                return operation;
            }).Produces<Game>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);


        return app;
    }
}