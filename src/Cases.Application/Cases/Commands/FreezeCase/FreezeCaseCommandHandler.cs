using Cases.Application.Cases.Interfaces;
using Cases.Application.Common.Interfaces;
using Cases.Domain.Exceptions;
using MediatR;

namespace Cases.Application.Cases.Commands.FreezeCase;

public sealed class FreezeCaseCommandHandler : IRequestHandler<FreezeCaseCommand, Unit>
{
    private readonly ICaseWriteRepository _cases;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public FreezeCaseCommandHandler(ICaseWriteRepository cases, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _cases = cases;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Unit> Handle(FreezeCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _cases.GetByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException("Case", request.CaseId);

        var now = _dateTimeProvider.UtcNow;
        caseEntity.SetActive(false, now);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
