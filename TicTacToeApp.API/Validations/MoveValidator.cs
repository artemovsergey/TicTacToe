using FluentValidation;
using TicTacToeApp.API.Entity;

namespace TicTacToeApp.API.Validations;

public class MoveValidator : AbstractValidator<Move>
{
    public MoveValidator()
    {
        RuleFor(m => m.x)
            .InclusiveBetween(0, 2)
            .WithMessage("Координата X должна быть между 0 и 2");
            
        RuleFor(m => m.y)
            .InclusiveBetween(0, 2)
            .WithMessage("Координата Y должна быть между 0 и 2");
            
        RuleFor(m => m.p)
            .IsInEnum()
            .WithMessage("Недопустимый игрок");
    }
}