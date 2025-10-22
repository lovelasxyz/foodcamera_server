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
}
