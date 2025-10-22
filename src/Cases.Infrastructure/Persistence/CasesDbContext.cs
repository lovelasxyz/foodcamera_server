using Cases.Application.Common.Interfaces;
using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cases.Infrastructure.Persistence;

public sealed class CasesDbContext(DbContextOptions<CasesDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Prize> Prizes => Set<Prize>();
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<CasePrize> CasePrizes => Set<CasePrize>();

    public Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        return Database.CanConnectAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CasesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
