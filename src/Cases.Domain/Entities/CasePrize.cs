using Cases.Domain.Common;

namespace Cases.Domain.Entities;

public sealed class CasePrize : BaseEntity<int>
{
    public int CaseId { get; private set; }
    public int PrizeId { get; private set; }
    public int Weight { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public Case Case { get; private set; } = null!;
    public Prize Prize { get; private set; } = null!;

    private CasePrize()
    {
    }
}
