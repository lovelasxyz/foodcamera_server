using Cases.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.NameTranslation;

namespace Cases.Infrastructure.Persistence.Extensions;

public static class PostgresEnumExtensions
{
    public static void MapPostgresEnums(this NpgsqlDataSourceBuilder builder)
    {
        var nameTranslator = new NpgsqlSnakeCaseNameTranslator();

        builder.MapEnum<PrizeRarity>("prize_rarity", nameTranslator);
        builder.MapEnum<BenefitType>("benefit_type", nameTranslator);
        builder.MapEnum<UserRole>("user_role", nameTranslator);
        builder.MapEnum<InventoryItemStatus>("inventory_item_status", nameTranslator);
    }

    public static void ConfigurePostgresEnums(this ModelBuilder modelBuilder)
    {
        // Регистрируем enum типы - NpgsqlSnakeCaseNameTranslator автоматически конвертирует:
        // PrizeRarity.Common -> "common", BenefitType.LotteryTicket -> "lottery_ticket" и т.д.
        modelBuilder.HasPostgresEnum<PrizeRarity>("prize_rarity");
        modelBuilder.HasPostgresEnum<BenefitType>("benefit_type");
        modelBuilder.HasPostgresEnum<UserRole>("user_role");
        modelBuilder.HasPostgresEnum<InventoryItemStatus>("inventory_item_status");
    }
}
