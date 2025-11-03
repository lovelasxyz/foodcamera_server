using Cases.Application.Cases.Interfaces;
using Cases.Application.Common.Interfaces;
using Cases.Domain.Exceptions;
using MediatR;

namespace Cases.Application.Cases.Commands.UpdateCase;

public sealed class UpdateCaseCommandHandler : IRequestHandler<UpdateCaseCommand, Unit>
{
    private readonly ICaseWriteRepository _cases;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateCaseCommandHandler(ICaseWriteRepository cases, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _cases = cases;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Unit> Handle(UpdateCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _cases.GetByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException("Case", request.CaseId);

        var name = request.NameSpecified ? request.Name : caseEntity.Name;
        var image = request.ImageSpecified ? request.Image : caseEntity.Image;

        var price = request.PriceSpecified
            ? request.Price ?? throw new InvalidInputException("Price must have a value when specified.")
            : caseEntity.Price;

        var commissionPercent = request.CommissionPercentSpecified
            ? request.CommissionPercent ?? throw new InvalidInputException("Commission percent must have a value when specified.")
            : caseEntity.CommissionPercent;

        var sortOrder = request.SortOrderSpecified
            ? request.SortOrder ?? throw new InvalidInputException("Sort order must have a value when specified.")
            : caseEntity.SortOrder;

        var autoHide = request.AutoHideSpecified
            ? request.AutoHide ?? throw new InvalidInputException("Auto hide must have a value when specified.")
            : caseEntity.AutoHide;

        var visibleFrom = request.VisibleFromSpecified
            ? request.VisibleFrom ?? throw new InvalidInputException("Visible from must have a value when specified.")
            : caseEntity.VisibleFrom;

        var visibleUntil = request.VisibleUntilSpecified
            ? request.VisibleUntil
            : caseEntity.VisibleUntil;

        caseEntity.UpdateDetails(
            name,
            image,
            price,
            commissionPercent,
            sortOrder,
            autoHide,
            visibleFrom,
            visibleUntil,
            _dateTimeProvider.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
