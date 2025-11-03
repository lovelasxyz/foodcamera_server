using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Domain.Entities;

namespace Cases.Application.Users.Interfaces;

public interface IUserWriteRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByTelegramIdAsync(string telegramId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
