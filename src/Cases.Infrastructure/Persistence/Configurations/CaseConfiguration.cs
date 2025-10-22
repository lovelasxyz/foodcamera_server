using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cases.Infrastructure.Persistence.Configurations;

public sealed class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("cases");

        builder.HasKey(@case => @case.Id);

        builder.Property(@case => @case.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(@case => @case.Name)
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(@case => @case.Image)
            .HasColumnName("image");

        builder.Property(@case => @case.Price)
            .HasColumnName("price")
            .HasPrecision(10, 2);

        builder.Property(@case => @case.CommissionPercent)
            .HasColumnName("commission_percent")
            .HasPrecision(5, 2);

        builder.Property(@case => @case.IsActive)
            .HasColumnName("is_active");

        builder.Property(@case => @case.SortOrder)
            .HasColumnName("sort_order");

        builder.Property(@case => @case.Balance)
            .HasColumnName("balance")
            .HasPrecision(10, 2);

        builder.Property(@case => @case.VisibleFrom)
            .HasColumnName("visible_from");

        builder.Property(@case => @case.VisibleUntil)
            .HasColumnName("visible_until");

        builder.Property(@case => @case.AutoHide)
            .HasColumnName("auto_hide");

        builder.Property(@case => @case.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(@case => @case.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasMany(@case => @case.CasePrizes)
            .WithOne(cp => cp.Case)
            .HasForeignKey(cp => cp.CaseId);
    }
}
