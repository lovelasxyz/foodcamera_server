using FluentValidation;

namespace Cases.Application.Cases.Commands.RemovePrizeFromCase;

public sealed class RemovePrizeFromCaseCommandValidator : AbstractValidator<RemovePrizeFromCaseCommand>
{
    public RemovePrizeFromCaseCommandValidator()
    {
        RuleFor(x => x.CaseId)
            .GreaterThan(0);

        RuleFor(x => x.PrizeId)
            .GreaterThan(0);
    }
}
