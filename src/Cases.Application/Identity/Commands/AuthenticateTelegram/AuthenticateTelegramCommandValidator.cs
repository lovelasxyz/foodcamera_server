using FluentValidation;

namespace Cases.Application.Identity.Commands.AuthenticateTelegram;

public sealed class AuthenticateTelegramCommandValidator : AbstractValidator<AuthenticateTelegramCommand>
{
    public AuthenticateTelegramCommandValidator()
    {
        RuleFor(command => command.InitData)
            .NotEmpty()
            .WithMessage("Telegram init data must be provided.");
    }
}
