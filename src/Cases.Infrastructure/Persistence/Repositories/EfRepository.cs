using System.Linq;
using System.Linq.Expressions;
using Cases.Application.Common.Interfaces;
using Cases.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Cases.Infrastructure.Persistence.Repositories;

public sealed class EfRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IAggregateRoot
{
    private readonly CasesDbContext _dbContext;

    public EfRepository(CasesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
    }
}
