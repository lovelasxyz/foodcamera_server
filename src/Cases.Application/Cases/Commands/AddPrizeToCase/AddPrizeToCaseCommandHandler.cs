using Cases.Application.Cases.Interfaces;
using Cases.Application.Common.Interfaces;
using Cases.Application.Prizes.Interfaces;
using Cases.Domain.Entities;
using Cases.Domain.Exceptions;
using MediatR;

namespace Cases.Application.Cases.Commands.AddPrizeToCase;

public sealed class AddPrizeToCaseCommandHandler : IRequestHandler<AddPrizeToCaseCommand, int>
{
    private readonly ICaseWriteRepository _cases;
    private readonly IPrizeReadRepository _prizes;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AddPrizeToCaseCommandHandler(
        ICaseWriteRepository cases,
        IPrizeReadRepository prizes,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _cases = cases;
        _prizes = prizes;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<int> Handle(AddPrizeToCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _cases.GetByIdWithPrizesAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException("Case", request.CaseId);

        var prize = await _prizes.GetByIdAsync(request.PrizeId, cancellationToken);

        if (prize is null)
        {
            throw new NotFoundException("Prize", request.PrizeId);
        }

        var now = _dateTimeProvider.UtcNow;
        var casePrize = caseEntity.AddPrize(request.PrizeId, request.Weight, now);

        await _cases.AddPrizeAsync(casePrize, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return casePrize.Id;
    }
}
