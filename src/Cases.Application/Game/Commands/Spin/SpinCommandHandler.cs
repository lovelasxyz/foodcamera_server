using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Models;
using Cases.Application.Cases.Interfaces;
using Cases.Application.Users.Interfaces;
using Cases.Application.Inventory.Interfaces;
using Cases.Domain.Entities;
using MediatR;

namespace Cases.Application.Game.Commands.Spin;

public sealed class SpinCommandHandler : IRequestHandler<SpinCommand, SpinResultDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICaseReadRepository _cases;
    private readonly IUserWriteRepository _users;
    private readonly IUserInventoryRepository _inventory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SpinCommandHandler(
        ICurrentUserService currentUserService,
        ICaseReadRepository cases,
        IUserWriteRepository users,
        IUserInventoryRepository inventory,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _currentUserService = currentUserService;
        _cases = cases;
        _users = users;
        _inventory = inventory;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<SpinResultDto> Handle(SpinCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            throw new UnauthorizedAccessException();
        }

        var user = await _users.GetByIdAsync(userId.Value, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException();
        }

        if (!int.TryParse(request.CaseId, out var caseId))
        {
             throw new ArgumentException("Invalid Case ID");
        }

        var allCases = await _cases.GetWithPrizesAsync(true, cancellationToken);
        var gameCase = allCases.FirstOrDefault(c => c.Id == caseId);

        if (gameCase == null)
        {
            throw new KeyNotFoundException($"Case {caseId} not found");
        }

        if (user.Balance < gameCase.Price)
        {
            throw new InvalidOperationException("Insufficient balance");
        }

        var now = _dateTimeProvider.UtcNow;
        user.Debit(gameCase.Price, now);

        var prize = SelectPrize(gameCase.CasePrizes);
        
        var inventoryItem = UserInventoryItem.Create(
            user.Id,
            Guid.NewGuid().ToString(),
            prize.Id,
            now.ToUnixTimeSeconds(),
            gameCase.Name,
            now
        );
        
        await _inventory.AddAsync(inventoryItem, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var prizeDto = new PrizeDto(
            prize.Id.ToString(),
            prize.Name ?? "Unknown",
            "item", 
            prize.Rarity.ToString(),
            prize.Image,
            prize.Price
        );

        var outcomeDto = new AwardOutcomeDto(true, "Success", null);
        
        var inventoryItems = await _inventory.GetByUserIdAsync(user.Id, cancellationToken);
        var inventoryDtos = inventoryItems.Select(i => new InventoryItemDto(
            i.InventoryItemId,
            i.PrizeId.ToString(),
            i.Status.ToString(),
            i.CreatedAt,
            new PrizeDto(
                i.Prize.Id.ToString(),
                i.Prize.Name ?? "Unknown",
                "item",
                i.Prize.Rarity.ToString(),
                i.Prize.Image,
                i.Prize.Price
            )
        ));

        return new SpinResultDto(
            prizeDto,
            outcomeDto,
            user.Balance,
            inventoryDtos
        );
    }

    private Prize SelectPrize(IEnumerable<CasePrize> casePrizes)
    {
        var prizes = casePrizes.ToList();
        if (!prizes.Any()) throw new InvalidOperationException("Case has no prizes");

        var totalWeight = prizes.Sum(p => p.Weight);
        var randomValue = new Random().Next(0, totalWeight);
        
        var currentWeight = 0;
        foreach (var casePrize in prizes)
        {
            currentWeight += casePrize.Weight;
            if (randomValue < currentWeight)
            {
                return casePrize.Prize;
            }
        }
        
        return prizes.Last().Prize;
    }
}
