using FluentValidation;

namespace Cases.Application.Cases.Commands.DeleteCase;

public sealed class DeleteCaseCommandValidator : AbstractValidator<DeleteCaseCommand>
{
    public DeleteCaseCommandValidator()
    {
        RuleFor(x => x.CaseId)
            .GreaterThan(0);
    }
}
