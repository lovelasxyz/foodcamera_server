using System;
using System.Collections.Generic;
using Cases.Domain.Common;
using Cases.Domain.Enums;

namespace Cases.Domain.Entities;

public sealed class Prize : AggregateRoot<int>
{
    public string? Name { get; private set; }
    public decimal Price { get; private set; }
    public string? Image { get; private set; }
    public PrizeRarity Rarity { get; private set; } = PrizeRarity.Common;
    public bool IsShard { get; private set; }
    public string? ShardKey { get; private set; }
    public int? ShardsRequired { get; private set; }
    public string? Description { get; private set; }
    public string? UniqueKey { get; private set; }
    public bool Stackable { get; private set; }
    public bool NotAwardIfOwned { get; private set; }
    public bool NonRemovableGift { get; private set; }
    public BenefitType? BenefitType { get; private set; }
    public string? BenefitDataJson { get; private set; }
    public decimal DropWeight { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private readonly List<CasePrize> _casePrizes = new();
    public IReadOnlyCollection<CasePrize> CasePrizes => _casePrizes.AsReadOnly();

    private Prize()
    {
    }

    public static Prize Create(
        string? name,
        decimal price,
    string? image,
    PrizeRarity rarity,
        bool isShard,
        string? shardKey,
        int? shardsRequired,
        string? description,
        string? uniqueKey,
        bool stackable,
        bool notAwardIfOwned,
        bool nonRemovableGift,
    BenefitType? benefitType,
        string? benefitDataJson,
        decimal dropWeight,
        DateTimeOffset createdAt)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price));
        }

        if (dropWeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dropWeight));
        }

        return new Prize
        {
            Name = name,
            Price = price,
            Image = image,
            Rarity = rarity,
            IsShard = isShard,
            ShardKey = shardKey,
            ShardsRequired = shardsRequired,
            Description = description,
            UniqueKey = uniqueKey,
            Stackable = stackable,
            NotAwardIfOwned = notAwardIfOwned,
            NonRemovableGift = nonRemovableGift,
            BenefitType = benefitType,
            BenefitDataJson = benefitDataJson,
            DropWeight = dropWeight,
            IsActive = true,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
    }

    public void Update(
        string? name,
        decimal price,
    string? image,
    PrizeRarity rarity,
        bool isShard,
        string? shardKey,
        int? shardsRequired,
        string? description,
        string? uniqueKey,
        bool stackable,
        bool notAwardIfOwned,
        bool nonRemovableGift,
    BenefitType? benefitType,
        string? benefitDataJson,
        decimal dropWeight,
        bool isActive,
        DateTimeOffset now)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price));
        }

        if (dropWeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dropWeight));
        }

        Name = name;
        Price = price;
    Image = image;
    Rarity = rarity;
        IsShard = isShard;
        ShardKey = shardKey;
        ShardsRequired = shardsRequired;
        Description = description;
        UniqueKey = uniqueKey;
        Stackable = stackable;
        NotAwardIfOwned = notAwardIfOwned;
        NonRemovableGift = nonRemovableGift;
        BenefitType = benefitType;
        BenefitDataJson = benefitDataJson;
        DropWeight = dropWeight;
        IsActive = isActive;
        UpdatedAt = now;
    }

    public void Deactivate(DateTimeOffset now)
    {
        IsActive = false;
        UpdatedAt = now;
    }
}
