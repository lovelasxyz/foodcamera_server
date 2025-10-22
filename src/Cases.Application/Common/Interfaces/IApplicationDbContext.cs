using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cases.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Prize> Prizes { get; }
    DbSet<Case> Cases { get; }
    DbSet<CasePrize> CasePrizes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
}
