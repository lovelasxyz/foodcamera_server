using System.Linq;
using Cases.Application.Common.Interfaces;
using Cases.Domain.Common;
using Cases.Domain.Entities;
using Cases.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cases.Infrastructure.Persistence;

public sealed class CasesDbContext : DbContext, IUnitOfWork
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public CasesDbContext(
        DbContextOptions<CasesDbContext> options,
        IDomainEventDispatcher? domainEventDispatcher = null)
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher ?? NullDomainEventDispatcher.Instance;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Prize> Prizes => Set<Prize>();
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<CasePrize> CasePrizes => Set<CasePrize>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<UserInventoryItem> UserInventoryItems => Set<UserInventoryItem>();

    public Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        return Database.CanConnectAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await DispatchDomainEventsAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var result = base.SaveChanges(acceptAllChangesOnSuccess);
        DispatchDomainEventsAsync(CancellationToken.None).GetAwaiter().GetResult();
        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigurePostgresEnums();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CasesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEventEntries = ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0)
            .ToArray();

        if (domainEventEntries.Length == 0)
        {
            return;
        }

        var domainEvents = domainEventEntries
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToArray();

        foreach (var entry in domainEventEntries)
        {
            entry.Entity.ClearDomainEvents();
        }

        await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken).ConfigureAwait(false);
    }
}
