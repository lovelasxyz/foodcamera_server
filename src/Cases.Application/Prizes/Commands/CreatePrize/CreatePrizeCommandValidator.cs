using Cases.Domain.Enums;
using FluentValidation;

namespace Cases.Application.Prizes.Commands.CreatePrize;

public sealed class CreatePrizeCommandValidator : AbstractValidator<CreatePrizeCommand>
{
    public CreatePrizeCommandValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DropWeight)
            .GreaterThan(0);

        RuleFor(x => x.Rarity)
            .NotEmpty()
            .Must(BeValidRarity)
            .WithMessage("Invalid prize rarity value.");

        RuleFor(x => x.BenefitType)
            .Must(BeValidBenefitType)
            .When(x => !string.IsNullOrWhiteSpace(x.BenefitType))
            .WithMessage("Invalid benefit type value.");
    }

    private static bool BeValidRarity(string rarity)
    {
        try
        {
            _ = EnumMappings.ParsePrizeRarity(rarity);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool BeValidBenefitType(string? benefitType)
    {
        try
        {
            _ = EnumMappings.ParseBenefitType(benefitType);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
