using Cases.Application.Cases.Interfaces;
using Cases.Application.Common.Interfaces;
using Cases.Domain.Entities;
using MediatR;

namespace Cases.Application.Cases.Commands.CreateCase;

public sealed class CreateCaseCommandHandler : IRequestHandler<CreateCaseCommand, int>
{
    private readonly ICaseWriteRepository _cases;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateCaseCommandHandler(ICaseWriteRepository cases, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _cases = cases;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<int> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;
        var visibleFrom = request.VisibleFrom ?? now;

        var @case = Case.Create(
            request.Name,
            request.Image,
            request.Price,
            request.CommissionPercent,
            request.SortOrder,
            request.AutoHide,
            visibleFrom,
            request.VisibleUntil,
            now);

        await _cases.AddAsync(@case, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return @case.Id;
    }
}
