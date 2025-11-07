using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces;

namespace Cases.Application.Common;

public sealed class NullCasesChangeNotifier : ICasesChangeNotifier
{
    public static readonly ICasesChangeNotifier Instance = new NullCasesChangeNotifier();

    private NullCasesChangeNotifier()
    {
    }

    public Task NotifyCasesUpdatedAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task NotifyCaseDeletedAsync(int caseId, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
