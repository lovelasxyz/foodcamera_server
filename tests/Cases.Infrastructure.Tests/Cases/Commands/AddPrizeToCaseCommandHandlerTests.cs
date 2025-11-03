using System;
using Cases.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Commands.AddPrizeToCase;
using Cases.Application.Prizes.Commands.CreatePrize;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cases.Infrastructure.Tests.Cases.Commands;

public sealed class AddPrizeToCaseCommandHandlerTests
{
    [Fact]
    public async Task Handle_AddsPrizeToCase_WhenEntitiesExist()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var now = new DateTimeOffset(2024, 03, 02, 10, 0, 0, TimeSpan.Zero);

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

    var repository = new CaseRepository(dbContext);
    var prizeRepository = new PrizeReadRepository(dbContext);
    var handler = new AddPrizeToCaseCommandHandler(repository, prizeRepository, dbContext, new TestDateTimeProvider(now));

        var command = new AddPrizeToCaseCommand(caseEntity.Id, prize.Id, 50);

        var casePrizeId = await handler.Handle(command, CancellationToken.None);

        casePrizeId.Should().BeGreaterThan(0);

        var casePrize = await dbContext.CasePrizes.SingleAsync();
        casePrize.CaseId.Should().Be(caseEntity.Id);
        casePrize.PrizeId.Should().Be(prize.Id);
        casePrize.Weight.Should().Be(50);

        var updatedCase = await dbContext.Cases.Include(c => c.CasePrizes).FirstAsync();
        updatedCase.CasePrizes.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_Throws_WhenPrizeAlreadyLinked()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var now = new DateTimeOffset(2024, 03, 02, 11, 0, 0, TimeSpan.Zero);

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

        var dateProvider = new TestDateTimeProvider(now);
    var repository = new CaseRepository(dbContext);
    var prizeRepository = new PrizeReadRepository(dbContext);
    var handler = new AddPrizeToCaseCommandHandler(repository, prizeRepository, dbContext, dateProvider);

        await handler.Handle(new AddPrizeToCaseCommand(caseEntity.Id, prize.Id, 50), CancellationToken.None);

        await FluentActions.Invoking(() => handler.Handle(new AddPrizeToCaseCommand(caseEntity.Id, prize.Id, 60), CancellationToken.None))
            .Should()
            .ThrowAsync<DuplicateException>()
            .WithMessage($"Prize {prize.Id} already added to case {caseEntity.Id}.");
    }
}
