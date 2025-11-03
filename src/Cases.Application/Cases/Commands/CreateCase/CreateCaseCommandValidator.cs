using FluentValidation;

namespace Cases.Application.Cases.Commands.CreateCase;

public sealed class CreateCaseCommandValidator : AbstractValidator<CreateCaseCommand>
{
    public CreateCaseCommandValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.CommissionPercent)
            .InclusiveBetween(0, 100);

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.VisibleUntil)
            .GreaterThan(x => x.VisibleFrom)
            .When(x => x.VisibleUntil is not null && x.VisibleFrom is not null);
    }
}
