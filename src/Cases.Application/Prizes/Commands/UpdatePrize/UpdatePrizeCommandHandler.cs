using Cases.Application.Common.Interfaces;
using Cases.Application.Prizes.Interfaces;
using Cases.Domain.Enums;
using Cases.Domain.Exceptions;
using MediatR;

namespace Cases.Application.Prizes.Commands.UpdatePrize;

public sealed class UpdatePrizeCommandHandler : IRequestHandler<UpdatePrizeCommand, Unit>
{
    private readonly IPrizeWriteRepository _prizes;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdatePrizeCommandHandler(IPrizeWriteRepository prizes, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _prizes = prizes;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Unit> Handle(UpdatePrizeCommand request, CancellationToken cancellationToken)
    {
        var prize = await _prizes.GetByIdAsync(request.PrizeId, cancellationToken)
            ?? throw new NotFoundException("Prize", request.PrizeId);

        if (!string.IsNullOrWhiteSpace(request.UniqueKey))
        {
            var keyInUse = await _prizes.UniqueKeyExistsAsync(request.UniqueKey!, request.PrizeId, cancellationToken);

            if (keyInUse)
            {
                throw new DuplicateException($"Prize with unique key '{request.UniqueKey}' already exists.");
            }
        }

        var rarity = EnumMappings.ParsePrizeRarity(request.Rarity);
        var benefitType = EnumMappings.ParseBenefitType(request.BenefitType);

        prize.Update(
            request.Name,
            request.Price,
            request.Image,
            rarity,
            request.IsShard,
            request.ShardKey,
            request.ShardsRequired,
            request.Description,
            request.UniqueKey,
            request.Stackable,
            request.NotAwardIfOwned,
            request.NonRemovableGift,
            benefitType,
            request.BenefitDataJson,
            request.DropWeight,
            request.IsActive,
            _dateTimeProvider.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
