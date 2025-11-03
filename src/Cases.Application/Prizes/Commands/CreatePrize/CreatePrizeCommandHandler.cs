using Cases.Application.Common.Interfaces;
using Cases.Application.Prizes.Interfaces;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using Cases.Domain.Exceptions;
using MediatR;

namespace Cases.Application.Prizes.Commands.CreatePrize;

public sealed class CreatePrizeCommandHandler : IRequestHandler<CreatePrizeCommand, int>
{
    private readonly IPrizeWriteRepository _prizes;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreatePrizeCommandHandler(IPrizeWriteRepository prizes, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _prizes = prizes;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<int> Handle(CreatePrizeCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.UniqueKey))
        {
            var exists = await _prizes.UniqueKeyExistsAsync(request.UniqueKey!, null, cancellationToken);

            if (exists)
            {
                throw new DuplicateException($"Prize with unique key '{request.UniqueKey}' already exists.");
            }
        }

        var now = _dateTimeProvider.UtcNow;
        var rarity = EnumMappings.ParsePrizeRarity(request.Rarity);
        var benefitType = EnumMappings.ParseBenefitType(request.BenefitType);
        var prize = Prize.Create(
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
            now);

        await _prizes.AddAsync(prize, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return prize.Id;
    }
}
