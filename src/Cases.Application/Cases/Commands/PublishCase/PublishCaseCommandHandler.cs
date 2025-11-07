using Cases.Application.Cases.Interfaces;
using Cases.Application.Common;
using Cases.Application.Common.Interfaces;
using Cases.Domain.Exceptions;
using MediatR;

namespace Cases.Application.Cases.Commands.PublishCase;

public sealed class PublishCaseCommandHandler : IRequestHandler<PublishCaseCommand, Unit>
{
    private readonly ICaseWriteRepository _cases;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICasesChangeNotifier _changeNotifier;

    public PublishCaseCommandHandler(
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

    public async Task<Unit> Handle(PublishCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _cases.GetByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException("Case", request.CaseId);

        var now = _dateTimeProvider.UtcNow;
        var visibleFrom = request.VisibleFrom ?? now;

        caseEntity.UpdateVisibility(visibleFrom, request.VisibleUntil, now);
        caseEntity.SetActive(true, now);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _changeNotifier.NotifyCasesUpdatedAsync(cancellationToken);

        return Unit.Value;
    }
}
