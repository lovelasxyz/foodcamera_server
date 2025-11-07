using Cases.Application.Cases.Interfaces;
using Cases.Application.Common;
using Cases.Application.Common.Interfaces;
using Cases.Domain.Exceptions;
using MediatR;

namespace Cases.Application.Cases.Commands.FreezeCase;

public sealed class FreezeCaseCommandHandler : IRequestHandler<FreezeCaseCommand, Unit>
{
    private readonly ICaseWriteRepository _cases;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICasesChangeNotifier _changeNotifier;

    public FreezeCaseCommandHandler(
    ICaseWriteRepository cases,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ICasesChangeNotifier? changeNotifier = null)
    {
        _cases = cases;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    _changeNotifier = changeNotifier ?? NullCasesChangeNotifier.Instance;
    }

    public async Task<Unit> Handle(FreezeCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _cases.GetByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException("Case", request.CaseId);

        var now = _dateTimeProvider.UtcNow;
        caseEntity.SetActive(false, now);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _changeNotifier.NotifyCasesUpdatedAsync(cancellationToken);

        return Unit.Value;
    }
}
