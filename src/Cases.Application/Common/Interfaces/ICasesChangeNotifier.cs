using System.Threading;
using System.Threading.Tasks;

namespace Cases.Application.Common.Interfaces;

public interface ICasesChangeNotifier
{
    Task NotifyCasesUpdatedAsync(CancellationToken cancellationToken = default);
    Task NotifyCaseDeletedAsync(int caseId, CancellationToken cancellationToken = default);
}
