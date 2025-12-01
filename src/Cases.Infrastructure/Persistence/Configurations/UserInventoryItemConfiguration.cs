using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cases.Infrastructure.Persistence.Configurations;

public sealed class UserInventoryItemConfiguration : IEntityTypeConfiguration<UserInventoryItem>
{
    public void Configure(EntityTypeBuilder<UserInventoryItem> builder)
    {
        builder.ToTable("user_inventory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.InventoryItemId)
            .HasColumnName("inventory_item_id")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.PrizeId)
            .HasColumnName("prize_id")
            .IsRequired();

        builder.Property(x => x.ObtainedAt)
            .HasColumnName("obtained_at");

        builder.Property(x => x.FromCase)
            .HasColumnName("from_case");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("inventory_item_status");

        builder.Property(x => x.ActivationCode)
            .HasColumnName("activation_code");

        builder.Property(x => x.ActivatedAt)
            .HasColumnName("activated_at");

        builder.Property(x => x.SelectedCaseId)
            .HasColumnName("selected_case_id");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Prize)
            .WithMany()
            .HasForeignKey(x => x.PrizeId);
            
        builder.HasOne(x => x.SelectedCase)
            .WithMany()
            .HasForeignKey(x => x.SelectedCaseId);
    }
}
