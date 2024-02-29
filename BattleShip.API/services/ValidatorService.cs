using BattleShip.API.data;
using FluentValidation;

public class AttackRequestValidator : AbstractValidator<AttackRequest>
{
    public AttackRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.GridSize).Must(BeValidGridSize).WithMessage("GridSize should be 10 or 15");
    }

    private bool BeValidGridSize(int gridSize)
    {
        return gridSize == 10 || gridSize == 15;
    }
}

