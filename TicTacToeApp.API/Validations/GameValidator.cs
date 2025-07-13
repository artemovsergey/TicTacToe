using FluentValidation;
using TicTacToeApp.API.Entity;
using TicTacToeApp.API.Entity.Enums;

namespace TicTacToeApp.API.Validations;

public class GameValidator : AbstractValidator<Game>
{
    public GameValidator(Move move)
    {
        RuleFor(g => g.Status)
            .Equal(StatusGame.Active)
            .WithMessage(g => $"Данная игра уже завершена! Итог: {g.Result}");
            
        RuleFor(g => g.CurrentMove)
            .Must((game, currentMove) => game.CurrentMove == currentMove)
            .WithMessage(g => $"Не ваш ход! Сейчас ход: {g.CurrentMove}");
            
        RuleFor(g => g.Board)
            .Must((game, board) => board[move.x][move.y] == null)
            .When((game, move) => move != null)
            .WithMessage("Нельзя осуществить данный ход! Ячейка занята!");
    }
}