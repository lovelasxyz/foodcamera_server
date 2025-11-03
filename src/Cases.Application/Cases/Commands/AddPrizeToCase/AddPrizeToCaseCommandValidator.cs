using FluentValidation;

namespace Cases.Application.Cases.Commands.AddPrizeToCase;

public sealed class AddPrizeToCaseCommandValidator : AbstractValidator<AddPrizeToCaseCommand>
{
    public AddPrizeToCaseCommandValidator()
    {
        RuleFor(x => x.CaseId)
            .GreaterThan(0);

        RuleFor(x => x.PrizeId)
            .GreaterThan(0);

        RuleFor(x => x.Weight)
            .GreaterThan(0);
    }
}
