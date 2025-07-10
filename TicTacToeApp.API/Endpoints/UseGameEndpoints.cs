using Microsoft.AspNetCore.Mvc;
using TicTacToeApp.API.Entity;
using TicTacToeApp.API.Entity.Enums;
using TicTacToeApp.API.Interfaces;
using TicTacToeApp.API.Services;

namespace TicTacToeApp.API.Endpoints;

public static class GameEndpoints
{
    const int TICTACTOE_BOARD_SIZE = 3;
    const int TICTACTOE_LINE_TO_WIN = 3;
    const int TICTACTOE_CHANCE = 50; // процент вероятности замены хода
    const int TICTACTOE_NUMBER_STEP = 2; // на каком ходу

    public static WebApplication UseGameEndpoints(
        this WebApplication app,
        CancellationToken cancellationToken
    )
    {
        app.MapGet("/api/games", (IGameRepository repo) => { return Results.Ok(repo.GetGames().Select(g => new { Id = g.Id })); });
        app.MapGet("/api/games/{Id}", (IGameRepository repo, Guid Id) => { return Results.Ok(repo.FindGameByGuid(Id)); });
        app.MapPost("/api/games/new", (IGameRepository repo) =>
        {
            if (TICTACTOE_LINE_TO_WIN > TICTACTOE_BOARD_SIZE)
                return Results.BadRequest(
                    "Количество одинаковых элементов должно быть меньше или равно размерности доски!");

            var game = repo.CreateGame(TICTACTOE_BOARD_SIZE);
            return Results.Ok(game);
        });

        app.MapPost("api/games/{game_id:guid}/move",
            (   HttpResponse r,
                [FromHeader(Name = "If-Match")] string? ifMatchHeader,
                IGameRepository repo,
                Guid game_id,
                Move move) =>
            {
                // Находим нужную игру
                var game = repo.FindGameByGuid(game_id);

                // var etag = EtagService.GenerateETag(game);

                // Проверка ETag
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

                if (game.CurrentStep > 0 && game.CurrentStep % 2 == 0)
                {
                    var maybeReplace = new Random().NextInt64(1, 3); // [1,2]
                    Console.WriteLine($"Рандомное число: {maybeReplace}");
                    if (maybeReplace == 2)
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

                repo.UpdateGame(game);

                // Формируем ответ 

                var response = new
                {
                    GameId = game_id,
                    Board = game.Board,
                    Status = game.Status,
                    Result = game.Result,
                    DateTime = DateTime.UtcNow,
                    CurrentStep = game.CurrentStep,
                    CurrentMove = game.CurrentMove
                };

                // Отправляем ответ c новым Еtag
                r.Headers.ETag = EtagService.GenerateETag(game);
                return Results.Ok(response);
            });

        return app;
    }
    
    

    
    
}