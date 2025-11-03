using FluentValidation;

namespace Cases.Application.Cases.Commands.FreezeCase;

public sealed class FreezeCaseCommandValidator : AbstractValidator<FreezeCaseCommand>
{
    public FreezeCaseCommandValidator()
    {
        RuleFor(x => x.CaseId)
            .GreaterThan(0);
    }
}
