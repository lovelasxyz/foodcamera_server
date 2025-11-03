using System;
using System.Collections.Generic;
using System.Linq;
using Cases.Domain.Common;

namespace Cases.Domain.Entities;

public sealed class Case : AggregateRoot<int>
{
    public string? Name { get; private set; }
    public string? Image { get; private set; }
    public decimal Price { get; private set; }
    public decimal CommissionPercent { get; private set; }
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
    public decimal Balance { get; private set; }
    public DateTimeOffset VisibleFrom { get; private set; }
    public DateTimeOffset? VisibleUntil { get; private set; }
    public bool AutoHide { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private readonly List<CasePrize> _casePrizes = new();
    public IReadOnlyCollection<CasePrize> CasePrizes => _casePrizes.AsReadOnly();

    private Case()
    {
    }

    public static Case Create(
        string? name,
        string? image,
        decimal price,
        decimal commissionPercent,
        int sortOrder,
        bool autoHide,
        DateTimeOffset visibleFrom,
        DateTimeOffset? visibleUntil,
        DateTimeOffset createdAt)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price));
        }

        if (commissionPercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(commissionPercent));
        }

        return new Case
        {
            Name = name,
            Image = image,
            Price = price,
            CommissionPercent = commissionPercent,
            IsActive = false,
            SortOrder = sortOrder,
            Balance = 0,
            VisibleFrom = visibleFrom,
            VisibleUntil = visibleUntil,
            AutoHide = autoHide,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
    }

    public void UpdateDetails(
        string? name,
        string? image,
        decimal price,
        decimal commissionPercent,
        int sortOrder,
        bool autoHide,
        DateTimeOffset visibleFrom,
        DateTimeOffset? visibleUntil,
        DateTimeOffset now)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price));
        }

        if (commissionPercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(commissionPercent));
        }

        Name = name;
        Image = image;
        Price = price;
        CommissionPercent = commissionPercent;
        SortOrder = sortOrder;
        AutoHide = autoHide;
        VisibleFrom = visibleFrom;
        VisibleUntil = visibleUntil;
        UpdatedAt = now;
    }

    public void SetActive(bool isActive, DateTimeOffset now)
    {
        IsActive = isActive;
        UpdatedAt = now;
    }

    public CasePrize AddPrize(int prizeId, int weight, DateTimeOffset now)
    {
        if (_casePrizes.Any(cp => cp.PrizeId == prizeId))
        {
            throw new Exceptions.DuplicateException($"Prize {prizeId} already added to case {Id}.");
        }

        var casePrize = CasePrize.Create(Id, prizeId, weight, now);
        casePrize.SetCase(this);
        _casePrizes.Add(casePrize);

        return casePrize;
    }

    public void RemovePrize(int prizeId)
    {
        var casePrize = _casePrizes.FirstOrDefault(cp => cp.PrizeId == prizeId);

        if (casePrize is null)
        {
            return;
        }

        _casePrizes.Remove(casePrize);
    }

    public void UpdatePrizeWeight(int prizeId, int weight, DateTimeOffset now)
    {
        var casePrize = _casePrizes.FirstOrDefault(cp => cp.PrizeId == prizeId)
            ?? throw new Exceptions.NotFoundException(nameof(CasePrize), prizeId);

        casePrize.UpdateWeight(weight);
        UpdatedAt = now;
    }

    public void UpdateVisibility(DateTimeOffset visibleFrom, DateTimeOffset? visibleUntil, DateTimeOffset now)
    {
        if (visibleUntil is not null && visibleUntil <= visibleFrom)
        {
            throw new ArgumentException("Visible until must be greater than visible from.", nameof(visibleUntil));
        }

        VisibleFrom = visibleFrom;
        VisibleUntil = visibleUntil;
        UpdatedAt = now;
    }

    public void AdjustBalance(decimal amount, DateTimeOffset now)
    {
        Balance += amount;
        UpdatedAt = now;
    }
}
