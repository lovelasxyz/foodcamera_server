using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cases.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(user => user.Name)
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(user => user.Balance)
            .HasColumnName("balance")
            .HasPrecision(10, 2);

        builder.Property(user => user.Role)
            .HasColumnName("role")
            .HasMaxLength(50)
            .HasDefaultValue("regular");

        builder.Property(user => user.TelegramId)
            .HasColumnName("telegram_id")
            .HasMaxLength(255);

        builder.Property(user => user.TelegramUsername)
            .HasColumnName("telegram_username")
            .HasMaxLength(255);

        builder.Property(user => user.TelegramRegisteredAt)
            .HasColumnName("telegram_registered_at");

        builder.Property(user => user.TelegramHasPhoto)
            .HasColumnName("telegram_has_photo");

        builder.Property(user => user.TelegramPhotoUrl)
            .HasColumnName("telegram_photo_url");

        builder.Property(user => user.FirstDepositAt)
            .HasColumnName("first_deposit_at");

        builder.Property(user => user.LastDepositAt)
            .HasColumnName("last_deposit_at");

        builder.Property(user => user.LastAuthAt)
            .HasColumnName("last_auth_at");

        builder.Property(user => user.LastSpinAt)
            .HasColumnName("last_spin_at");

        builder.Property(user => user.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(user => user.UpdatedAt)
            .HasColumnName("updated_at");
    }
}
