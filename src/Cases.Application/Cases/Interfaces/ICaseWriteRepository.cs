using System.Threading;
using System.Threading.Tasks;
using Cases.Domain.Entities;

namespace Cases.Application.Cases.Interfaces;

public interface ICaseWriteRepository
{
    Task AddAsync(Case caseEntity, CancellationToken cancellationToken = default);
    Task<Case?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Case?> GetByIdWithPrizesAsync(int id, CancellationToken cancellationToken = default);
    void Remove(Case caseEntity);
    Task AddPrizeAsync(CasePrize casePrize, CancellationToken cancellationToken = default);
    Task<CasePrize?> GetCasePrizeAsync(int caseId, int prizeId, CancellationToken cancellationToken = default);
    void RemovePrize(CasePrize casePrize);
}