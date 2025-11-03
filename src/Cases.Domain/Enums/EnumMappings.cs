using System;

namespace Cases.Domain.Enums;

public static class EnumMappings
{
    public static PrizeRarity ParsePrizeRarity(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return PrizeRarity.Common;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "common" => PrizeRarity.Common,
            "rare" => PrizeRarity.Rare,
            "epic" => PrizeRarity.Epic,
            "legendary" => PrizeRarity.Legendary,
            _ => throw new ArgumentException($"Unknown prize rarity '{value}'.", nameof(value))
        };
    }

    public static BenefitType? ParseBenefitType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "discount" => BenefitType.Discount,
            "subscription" => BenefitType.Subscription,
            "lottery_ticket" => BenefitType.LotteryTicket,
            "bigwin" => BenefitType.Bigwin,
            "fiat_usdt" => BenefitType.FiatUsdt,
            "weekly_ticket" => BenefitType.WeeklyTicket,
            "permanent_token" => BenefitType.PermanentToken,
            _ => throw new ArgumentException($"Unknown benefit type '{value}'.", nameof(value))
        };
    }

    public static UserRole ParseUserRole(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("User role value is required.", nameof(value));
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "admin" => UserRole.Admin,
            "regular" => UserRole.Regular,
            "premium" => UserRole.Premium,
            "advertiser" => UserRole.Advertiser,
            _ => throw new ArgumentException($"Unknown user role '{value}'.", nameof(value))
        };
    }

    public static string ToDatabaseValue(this PrizeRarity rarity) => rarity switch
    {
        PrizeRarity.Common => "common",
        PrizeRarity.Rare => "rare",
        PrizeRarity.Epic => "epic",
        PrizeRarity.Legendary => "legendary",
        _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
    };

    public static string? ToDatabaseValue(this BenefitType? benefitType) => benefitType switch
    {
        null => null,
        BenefitType.Discount => "discount",
        BenefitType.Subscription => "subscription",
        BenefitType.LotteryTicket => "lottery_ticket",
        BenefitType.Bigwin => "bigwin",
        BenefitType.FiatUsdt => "fiat_usdt",
        BenefitType.WeeklyTicket => "weekly_ticket",
        BenefitType.PermanentToken => "permanent_token",
        _ => throw new ArgumentOutOfRangeException(nameof(benefitType), benefitType, null)
    };

    public static string ToDatabaseValue(this UserRole role) => role switch
    {
        UserRole.Admin => "admin",
        UserRole.Regular => "regular",
        UserRole.Premium => "premium",
        UserRole.Advertiser => "advertiser",
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
    };
}
