using BattleShip.API.data;
using FluentValidation;

public class AttackRequestValidator : AbstractValidator<AttackRequestGrpc>
{
    public AttackRequestValidator()
    {
        RuleFor(x => x.GameId).NotEmpty();
    }
}

