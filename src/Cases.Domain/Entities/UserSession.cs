using System;
using Cases.Domain.Common;

namespace Cases.Domain.Entities;

public sealed class UserSession : BaseEntity<Guid>
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset LastUsedAt { get; private set; }

    private UserSession()
    {
        // Required by EF Core
    }

    public static UserSession Create(User user, DateTimeOffset createdAt, DateTimeOffset expiresAt)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            CreatedAt = createdAt,
            LastUsedAt = createdAt,
            ExpiresAt = expiresAt
        };
    }

    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;

    public void Refresh(DateTimeOffset now, DateTimeOffset newExpiresAt)
    {
        LastUsedAt = now;
        ExpiresAt = newExpiresAt;
    }
}