using FluentValidation;

namespace Cases.Application.Cases.Commands.PublishCase;

public sealed class PublishCaseCommandValidator : AbstractValidator<PublishCaseCommand>
{
    public PublishCaseCommandValidator()
    {
        RuleFor(x => x.CaseId)
            .GreaterThan(0);

        RuleFor(x => x.VisibleUntil)
            .GreaterThan(x => x.VisibleFrom)
            .When(x => x.VisibleFrom.HasValue && x.VisibleUntil.HasValue);
    }
}
