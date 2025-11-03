using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cases.Infrastructure.Persistence.Configurations;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(session => session.Id);

        builder.Property(session => session.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(session => session.UserId)
            .HasColumnName("user_id");

        builder.Property(session => session.ExpiresAt)
            .HasColumnName("expires_at");

        builder.Property(session => session.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(session => session.LastUsedAt)
            .HasColumnName("last_used_at");

        builder.HasOne(session => session.User)
            .WithMany()
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(session => session.UserId)
            .HasDatabaseName("sessions_user_id_idx");

        builder.HasIndex(session => session.ExpiresAt)
            .HasDatabaseName("sessions_expires_at_idx");
    }
}