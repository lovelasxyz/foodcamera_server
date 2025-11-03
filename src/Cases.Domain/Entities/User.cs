using System.Linq;
using Cases.Domain.Common;
using Cases.Domain.Enums;

namespace Cases.Domain.Entities;

public sealed class User : AggregateRoot<Guid>
{
    public string? Name { get; private set; }
    public decimal Balance { get; private set; }
    public UserRole Role { get; private set; } = UserRole.Regular;
    public string? TelegramId { get; private set; }
    public string? TelegramUsername { get; private set; }
    public long? TelegramRegisteredAt { get; private set; }
    public bool TelegramHasPhoto { get; private set; }
    public string? TelegramPhotoUrl { get; private set; }
    public DateTimeOffset? FirstDepositAt { get; private set; }
    public DateTimeOffset? LastDepositAt { get; private set; }
    public long? LastAuthAt { get; private set; }
    public DateTimeOffset? LastSpinAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private User()
    {
        // Required by EF Core
    }

    public static User CreateFromTelegram(
        string telegramId,
        string? firstName,
        string? lastName,
        string? username,
        string? photoUrl,
        long authDate,
        DateTimeOffset now)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = BuildDisplayName(firstName, lastName),
            Balance = 0,
            Role = UserRole.Regular,
            TelegramId = telegramId,
            TelegramUsername = username,
            TelegramRegisteredAt = authDate,
            TelegramHasPhoto = !string.IsNullOrWhiteSpace(photoUrl),
            TelegramPhotoUrl = photoUrl,
            LastAuthAt = authDate,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void UpdateTelegramProfile(
        string? firstName,
        string? lastName,
        string? username,
        string? photoUrl,
        long authDate,
        DateTimeOffset now)
    {
        Name = BuildDisplayName(firstName, lastName);
        TelegramUsername = username;
        TelegramHasPhoto = !string.IsNullOrWhiteSpace(photoUrl);
        TelegramPhotoUrl = photoUrl;
        LastAuthAt = authDate;
        TelegramRegisteredAt ??= authDate;
        UpdatedAt = now;
    }

    private static string? BuildDisplayName(string? firstName, string? lastName)
    {
        var parts = new[] { firstName, lastName }
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .ToArray();

        return parts.Length == 0 ? null : string.Join(' ', parts);
    }
}
