using Cases.Application.Cases.Interfaces;
using Cases.Application.Common.Interfaces;
using MediatR;

namespace Cases.Application.Cases.Commands.RemovePrizeFromCase;

public sealed class RemovePrizeFromCaseCommandHandler : IRequestHandler<RemovePrizeFromCaseCommand, Unit>
{
    private readonly ICaseWriteRepository _cases;
    private readonly IUnitOfWork _unitOfWork;

    public RemovePrizeFromCaseCommandHandler(ICaseWriteRepository cases, IUnitOfWork unitOfWork)
    {
        _cases = cases;
        _unitOfWork = unitOfWork;
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

        return Unit.Value;
    }
}
