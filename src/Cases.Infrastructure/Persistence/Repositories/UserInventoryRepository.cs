using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Inventory.Interfaces;
using Cases.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cases.Infrastructure.Persistence.Repositories;

public sealed class UserInventoryRepository : IUserInventoryRepository
{
    private readonly CasesDbContext _context;

    public UserInventoryRepository(CasesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserInventoryItem item, CancellationToken cancellationToken = default)
    {
        await _context.UserInventoryItems.AddAsync(item, cancellationToken);
    }

    public async Task<IReadOnlyList<UserInventoryItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserInventoryItems
            .Where(x => x.UserId == userId)
            .Include(x => x.Prize)
            .ToListAsync(cancellationToken);
    }
}
