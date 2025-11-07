using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Cases.Infrastructure.RealTime;

public sealed class SignalRCasesChangeNotifier : ICasesChangeNotifier
{
    private readonly IHubContext<CasesHub> _hubContext;

    public SignalRCasesChangeNotifier(IHubContext<CasesHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyCasesUpdatedAsync(CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.All.SendAsync(
            "CasesUpdated",
            DateTimeOffset.UtcNow,
            cancellationToken);
    }

    public Task NotifyCaseDeletedAsync(int caseId, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.All.SendAsync("CaseDeleted", caseId, cancellationToken);
    }

    // User-specific notifications
    public Task NotifyUserBalanceUpdatedAsync(string userId, decimal newBalance, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group($"user_{userId}").SendAsync(
            "BalanceUpdated",
            new { newBalance },
            cancellationToken);
    }

    public Task NotifyUserInventoryItemAddedAsync(string userId, object item, string fromCase, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group($"user_{userId}").SendAsync(
            "InventoryItemAdded",
            new { item, fromCase },
            cancellationToken);
    }

    public Task NotifyUserInventoryItemRemovedAsync(string userId, string itemId, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group($"user_{userId}").SendAsync(
            "InventoryItemRemoved",
            new { itemId },
            cancellationToken);
    }

    public Task NotifyUserStatsUpdatedAsync(string userId, object stats, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group($"user_{userId}").SendAsync(
            "UserStatsUpdated",
            stats,
            cancellationToken);
    }

    public Task NotifyCaseOpenedAsync(string userId, int caseId, object prize, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group($"user_{userId}").SendAsync(
            "CaseOpened",
            new { caseId, prize },
            cancellationToken);
    }

    public Task NotifySpinCompletedAsync(string userId, object spinResult, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group($"user_{userId}").SendAsync(
            "SpinCompleted",
            spinResult,
            cancellationToken);
    }
}
