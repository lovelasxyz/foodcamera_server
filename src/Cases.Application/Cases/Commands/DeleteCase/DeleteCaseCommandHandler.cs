using Cases.Application.Cases.Interfaces;
using Cases.Application.Common;
using Cases.Application.Common.Interfaces;
using Cases.Domain.Exceptions;
using MediatR;

namespace Cases.Application.Cases.Commands.DeleteCase;

public sealed class DeleteCaseCommandHandler : IRequestHandler<DeleteCaseCommand, Unit>
{
    private readonly ICaseWriteRepository _cases;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICasesChangeNotifier _changeNotifier;

    public DeleteCaseCommandHandler(
        ICaseWriteRepository cases,
        IUnitOfWork unitOfWork,
        ICasesChangeNotifier? changeNotifier = null)
    {
        _cases = cases;
        _unitOfWork = unitOfWork;
        _changeNotifier = changeNotifier ?? NullCasesChangeNotifier.Instance;
    }

    public async Task<Unit> Handle(DeleteCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _cases.GetByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException("Case", request.CaseId);

        _cases.Remove(caseEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _changeNotifier.NotifyCaseDeletedAsync(request.CaseId, cancellationToken);

        return Unit.Value;
    }
}
