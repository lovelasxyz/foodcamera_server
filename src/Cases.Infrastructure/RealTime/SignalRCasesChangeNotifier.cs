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
}
