using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Queries.GetCase;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using Cases.Domain.Exceptions;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Xunit;

namespace Cases.Infrastructure.Tests.Cases.Queries;

public sealed class GetCaseQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsDetailedCaseWithPrizes()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var now = new DateTimeOffset(2024, 03, 06, 9, 0, 0, TimeSpan.Zero);

        var prizeA = Prize.Create(
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

        var prizeB = Prize.Create(
            name: "Epic Gem",
            price: 5,
            image: null,
            rarity: PrizeRarity.Epic,
            isShard: false,
            shardKey: null,
            shardsRequired: null,
            description: null,
            uniqueKey: "epic",
            stackable: false,
            notAwardIfOwned: false,
            nonRemovableGift: false,
            benefitType: null,
            benefitDataJson: null,
            dropWeight: 2,
            createdAt: now);

        dbContext.Prizes.AddRange(prizeA, prizeB);
        await dbContext.SaveChangesAsync();

        var caseEntity = Case.Create(
            name: "Starter Case",
            image: "image.png",
            price: 2,
            commissionPercent: 10,
            sortOrder: 0,
            autoHide: false,
            visibleFrom: now,
            visibleUntil: null,
            createdAt: now);

        caseEntity.SetActive(true, now);
        dbContext.Cases.Add(caseEntity);
        await dbContext.SaveChangesAsync();

        var casePrize1 = caseEntity.AddPrize(prizeA.Id, 70, now);
        var casePrize2 = caseEntity.AddPrize(prizeB.Id, 30, now);

        dbContext.CasePrizes.AddRange(casePrize1, casePrize2);
        await dbContext.SaveChangesAsync();

    var repository = new CaseReadRepository(dbContext);
    var handler = new GetCaseQueryHandler(repository);

        var result = await handler.Handle(new GetCaseQuery(caseEntity.Id), CancellationToken.None);

        result.Id.Should().Be(caseEntity.Id);
        result.Name.Should().Be("Starter Case");
        result.Prizes.Should().HaveCount(2);
        result.Prizes.Select(p => p.PrizeId).Should().BeEquivalentTo(new[] { prizeA.Id, prizeB.Id });
        result.Prizes.First(p => p.PrizeId == prizeA.Id).Weight.Should().Be(70);
        result.Prizes.First(p => p.PrizeId == prizeB.Id).PrizeRarity.Should().Be("epic");
    }

    [Fact]
    public async Task Handle_Throws_WhenCaseNotFound()
    {
    await using var dbContext = DbContextFactory.CreateInMemory();
    var repository = new CaseReadRepository(dbContext);
    var handler = new GetCaseQueryHandler(repository);

        await FluentActions.Invoking(() => handler.Handle(new GetCaseQuery(123), CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Case with id '123' was not found.");
    }
}
