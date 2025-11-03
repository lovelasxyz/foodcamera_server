using System;
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

    public static CasePrize Create(int caseId, int prizeId, int weight, DateTimeOffset createdAt)
    {
        if (weight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weight));
        }

        return new CasePrize
        {
            CaseId = caseId,
            PrizeId = prizeId,
            Weight = weight,
            CreatedAt = createdAt
        };
    }

    public void UpdateWeight(int weight)
    {
        if (weight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weight));
        }

        Weight = weight;
    }

    public void SetCase(Case @case)
    {
        Case = @case ?? throw new ArgumentNullException(nameof(@case));
        CaseId = @case.Id;
    }
}
