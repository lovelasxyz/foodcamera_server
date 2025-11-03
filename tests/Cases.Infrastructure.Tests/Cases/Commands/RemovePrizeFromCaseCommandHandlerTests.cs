using System;
using Cases.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Commands.RemovePrizeFromCase;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cases.Infrastructure.Tests.Cases.Commands;

public sealed class RemovePrizeFromCaseCommandHandlerTests
{
    [Fact]
    public async Task Handle_RemovesLink_WhenCasePrizeExists()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var now = new DateTimeOffset(2024, 03, 03, 12, 0, 0, TimeSpan.Zero);

        var prize = Prize.Create(
            name: "Gem",
            price: 1,
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
            dropWeight: 10,
            createdAt: now);

        dbContext.Prizes.Add(prize);
        await dbContext.SaveChangesAsync();

        var caseEntity = Case.Create(
            name: "Starter Case",
            image: null,
            price: 2,
            commissionPercent: 10,
            sortOrder: 0,
            autoHide: false,
            visibleFrom: now,
            visibleUntil: null,
            createdAt: now);

        dbContext.Cases.Add(caseEntity);
        await dbContext.SaveChangesAsync();

        var casePrize = caseEntity.AddPrize(prize.Id, 20, now);
        dbContext.CasePrizes.Add(casePrize);
        await dbContext.SaveChangesAsync();

    var repository = new CaseRepository(dbContext);
    var handler = new RemovePrizeFromCaseCommandHandler(repository, dbContext);

        await handler.Handle(new RemovePrizeFromCaseCommand(caseEntity.Id, prize.Id), CancellationToken.None);

        (await dbContext.CasePrizes.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Handle_DoesNothing_WhenCasePrizeMissing()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();

    var repository = new CaseRepository(dbContext);
    var handler = new RemovePrizeFromCaseCommandHandler(repository, dbContext);

        await handler.Handle(new RemovePrizeFromCaseCommand(999, 888), CancellationToken.None);

        (await dbContext.CasePrizes.CountAsync()).Should().Be(0);
    }
}
