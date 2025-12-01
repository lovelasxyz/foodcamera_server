using System;
using Cases.Domain.Common;
using Cases.Domain.Enums;

namespace Cases.Domain.Entities;

public sealed class UserInventoryItem : BaseEntity<Guid>
{
    public Guid UserId { get; private set; }
    public string InventoryItemId { get; private set; } = null!;
    public int PrizeId { get; private set; }
    public long ObtainedAt { get; private set; }
    public string? FromCase { get; private set; }
    public InventoryItemStatus Status { get; private set; } = InventoryItemStatus.Active;
    public string? ActivationCode { get; private set; }
    public DateTimeOffset? ActivatedAt { get; private set; }
    public int? SelectedCaseId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public User User { get; private set; } = null!;
    public Prize Prize { get; private set; } = null!;
    public Case? SelectedCase { get; private set; }

    private UserInventoryItem() { }

    public static UserInventoryItem Create(
        Guid userId,
        string inventoryItemId,
        int prizeId,
        long obtainedAt,
        string? fromCase,
        DateTimeOffset now)
    {
        return new UserInventoryItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            InventoryItemId = inventoryItemId,
            PrizeId = prizeId,
            ObtainedAt = obtainedAt,
            FromCase = fromCase,
            Status = InventoryItemStatus.Active,
            CreatedAt = now
        };
    }
}
