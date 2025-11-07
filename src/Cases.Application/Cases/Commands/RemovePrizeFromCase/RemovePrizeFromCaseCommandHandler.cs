using Cases.Application.Cases.Interfaces;
using Cases.Application.Common;
using Cases.Application.Common.Interfaces;
using MediatR;

namespace Cases.Application.Cases.Commands.RemovePrizeFromCase;

public sealed class RemovePrizeFromCaseCommandHandler : IRequestHandler<RemovePrizeFromCaseCommand, Unit>
{
    private readonly ICaseWriteRepository _cases;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICasesChangeNotifier _changeNotifier;

    public RemovePrizeFromCaseCommandHandler(
        ICaseWriteRepository cases,
        IUnitOfWork unitOfWork,
        ICasesChangeNotifier? changeNotifier = null)
    {
        _cases = cases;
        _unitOfWork = unitOfWork;
        _changeNotifier = changeNotifier ?? NullCasesChangeNotifier.Instance;
    }

    public async Task<Unit> Handle(RemovePrizeFromCaseCommand request, CancellationToken cancellationToken)
    {
        var casePrize = await _cases.GetCasePrizeAsync(request.CaseId, request.PrizeId, cancellationToken);

        if (casePrize is null)
        {
            return Unit.Value;
        }

        _cases.RemovePrize(casePrize);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _changeNotifier.NotifyCasesUpdatedAsync(cancellationToken);

        return Unit.Value;
    }
}
