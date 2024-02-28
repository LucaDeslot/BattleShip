using BattleShip.API.data;
using FluentValidation;

public class AttackRequestValidator : AbstractValidator<AttackRequest>
{
    public AttackRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.X).InclusiveBetween(0, 10);
        RuleFor(x => x.Y).InclusiveBetween(0, 10);
    }
}
