using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cases.Infrastructure.Persistence.Configurations;

public sealed class CasePrizeConfiguration : IEntityTypeConfiguration<CasePrize>
{
    public void Configure(EntityTypeBuilder<CasePrize> builder)
    {
        builder.ToTable("case_prizes");

        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(cp => cp.CaseId)
            .HasColumnName("case_id");

        builder.Property(cp => cp.PrizeId)
            .HasColumnName("prize_id");

        builder.Property(cp => cp.Weight)
            .HasColumnName("weight");

        builder.Property(cp => cp.CreatedAt)
            .HasColumnName("created_at");

        builder.HasOne(cp => cp.Case)
            .WithMany(@case => @case.CasePrizes)
            .HasForeignKey(cp => cp.CaseId);

        builder.HasOne(cp => cp.Prize)
            .WithMany(prize => prize.CasePrizes)
            .HasForeignKey(cp => cp.PrizeId);

        builder.HasIndex(cp => new { cp.CaseId, cp.PrizeId })
            .IsUnique()
            .HasDatabaseName("case_prizes_case_id_prize_id_key");
    }
}
