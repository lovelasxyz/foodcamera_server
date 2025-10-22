using Cases.Domain.Common;

namespace Cases.Domain.Entities;

public sealed class Prize : AggregateRoot<int>
{
    public string? Name { get; private set; }
    public decimal Price { get; private set; }
    public string? Image { get; private set; }
    public string Rarity { get; private set; } = "common";
    public bool IsShard { get; private set; }
    public string? ShardKey { get; private set; }
    public int? ShardsRequired { get; private set; }
    public string? Description { get; private set; }
    public string? UniqueKey { get; private set; }
    public bool Stackable { get; private set; }
    public bool NotAwardIfOwned { get; private set; }
    public bool NonRemovableGift { get; private set; }
    public string? BenefitType { get; private set; }
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
}
