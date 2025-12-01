using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cases.Domain.Entities;

namespace Cases.Application.Inventory.Interfaces;

public interface IUserInventoryRepository
{
    Task AddAsync(UserInventoryItem item, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserInventoryItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
