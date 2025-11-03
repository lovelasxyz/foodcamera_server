using Cases.Application.Cases.Interfaces;
using Cases.Application.Common.Interfaces;
using Cases.Domain.Exceptions;
using MediatR;

namespace Cases.Application.Cases.Commands.PublishCase;

public sealed class PublishCaseCommandHandler : IRequestHandler<PublishCaseCommand, Unit>
{
    private readonly ICaseWriteRepository _cases;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PublishCaseCommandHandler(ICaseWriteRepository cases, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _cases = cases;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
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

        return Unit.Value;
    }
}
