using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Commands.DeleteCase;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cases.Infrastructure.Tests.Cases.Commands;

public sealed class DeleteCaseCommandHandlerTests
{
    [Fact]
    public async Task Handle_RemovesCaseAndRelatedPrizes()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var now = new DateTimeOffset(2024, 03, 08, 10, 0, 0, TimeSpan.Zero);

        var prize = Prize.Create(
            name: "Gem",
            price: 1m,
            image: null,
            rarity: PrizeRarity.Common,
            isShard: false,
            shardKey: null,
            shardsRequired: null,
            description: null,
            uniqueKey: "gem",
            stackable: false,
            notAwardIfOwned: false,
            nonRemovableGift: false,
            benefitType: null,
            benefitDataJson: null,
            dropWeight: 10m,
            createdAt: now);

        var caseEntity = Case.Create(
            name: "Starter",
            image: "image.png",
            price: 5m,
            commissionPercent: 15m,
            sortOrder: 1,
            autoHide: false,
            visibleFrom: now,
            visibleUntil: null,
            createdAt: now);

        dbContext.Prizes.Add(prize);
        dbContext.Cases.Add(caseEntity);
        await dbContext.SaveChangesAsync();

        var link = caseEntity.AddPrize(prize.Id, 50, now);
        dbContext.CasePrizes.Add(link);
        await dbContext.SaveChangesAsync();

        var repository = new CaseRepository(dbContext);
        var handler = new DeleteCaseCommandHandler(repository, dbContext);

        await handler.Handle(new DeleteCaseCommand(caseEntity.Id), CancellationToken.None);

        (await dbContext.Cases.CountAsync()).Should().Be(0);
        (await dbContext.CasePrizes.CountAsync()).Should().Be(0);
    }
}
