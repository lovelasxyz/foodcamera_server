using FluentValidation;

namespace Cases.Application.Cases.Commands.UpdateCase;

public sealed class UpdateCaseCommandValidator : AbstractValidator<UpdateCaseCommand>
{
    public UpdateCaseCommandValidator()
    {
        RuleFor(x => x.CaseId)
            .GreaterThan(0);

        When(x => x.PriceSpecified, () =>
        {
            RuleFor(x => x.Price)
                .NotNull()
                .WithMessage("Price is required when provided.");

            RuleFor(x => x.Price!.Value)
                .GreaterThanOrEqualTo(0);
        });

        When(x => x.CommissionPercentSpecified, () =>
        {
            RuleFor(x => x.CommissionPercent)
                .NotNull()
                .WithMessage("Commission percent is required when provided.");

            RuleFor(x => x.CommissionPercent!.Value)
                .InclusiveBetween(0, 100);
        });

        When(x => x.SortOrderSpecified, () =>
        {
            RuleFor(x => x.SortOrder)
                .NotNull()
                .WithMessage("Sort order is required when provided.");

            RuleFor(x => x.SortOrder!.Value)
                .GreaterThanOrEqualTo(0);
        });

        When(x => x.AutoHideSpecified, () =>
        {
            RuleFor(x => x.AutoHide)
                .NotNull()
                .WithMessage("Auto hide is required when provided.");
        });

        When(x => x.VisibleFromSpecified, () =>
        {
            RuleFor(x => x.VisibleFrom)
                .NotNull()
                .WithMessage("Visible from is required when provided.");
        });

        When(x => x.VisibleUntilSpecified && x.VisibleUntil.HasValue, () =>
        {
            RuleFor(x => x.VisibleUntil!.Value)
                .Must((command, visibleUntil) =>
                {
                    var visibleFrom = command.VisibleFromSpecified
                        ? command.VisibleFrom
                        : null;

                    return visibleFrom is null || visibleUntil > visibleFrom;
                })
                .WithMessage("Visible until must be greater than visible from.");
        });
    }
}
