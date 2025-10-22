using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cases.Infrastructure.Persistence.Configurations;

public sealed class PrizeConfiguration : IEntityTypeConfiguration<Prize>
{
    public void Configure(EntityTypeBuilder<Prize> builder)
    {
        builder.ToTable("prizes");

        builder.HasKey(prize => prize.Id);

        builder.Property(prize => prize.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(prize => prize.Name)
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(prize => prize.Price)
            .HasColumnName("price")
            .HasPrecision(10, 2);

        builder.Property(prize => prize.Image)
            .HasColumnName("image");

        builder.Property(prize => prize.Rarity)
            .HasColumnName("rarity")
            .HasMaxLength(50);

        builder.Property(prize => prize.IsShard)
            .HasColumnName("is_shard");

        builder.Property(prize => prize.ShardKey)
            .HasColumnName("shard_key")
            .HasMaxLength(255);

        builder.Property(prize => prize.ShardsRequired)
            .HasColumnName("shards_required");

        builder.Property(prize => prize.Description)
            .HasColumnName("description");

        builder.Property(prize => prize.UniqueKey)
            .HasColumnName("unique_key")
            .HasMaxLength(255);

        builder.Property(prize => prize.Stackable)
            .HasColumnName("stackable");

        builder.Property(prize => prize.NotAwardIfOwned)
            .HasColumnName("not_award_if_owned");

        builder.Property(prize => prize.NonRemovableGift)
            .HasColumnName("non_removable_gift");

        builder.Property(prize => prize.BenefitType)
            .HasColumnName("benefit_type")
            .HasMaxLength(50);

        builder.Property(prize => prize.BenefitDataJson)
            .HasColumnName("benefit_data");

        builder.Property(prize => prize.DropWeight)
            .HasColumnName("drop_weight")
            .HasPrecision(10, 4);

        builder.Property(prize => prize.IsActive)
            .HasColumnName("is_active");

        builder.Property(prize => prize.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(prize => prize.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasMany(prize => prize.CasePrizes)
            .WithOne(cp => cp.Prize)
            .HasForeignKey(cp => cp.PrizeId);
    }
}
