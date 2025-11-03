using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Prizes.Queries.GetPrize;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Xunit;

namespace Cases.Infrastructure.Tests.Prizes.Queries;

public sealed class GetPrizeQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPrize_WhenExists()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var createdAt = new DateTimeOffset(2024, 03, 08, 10, 0, 0, TimeSpan.Zero);

        var prize = Prize.Create(
            name: "Gem",
            price: 1,
            image: "image.png",
            rarity: PrizeRarity.Common,
            isShard: false,
            shardKey: null,
            shardsRequired: null,
            description: "desc",
            uniqueKey: "gem",
            stackable: false,
            notAwardIfOwned: false,
            nonRemovableGift: false,
            benefitType: null,
            benefitDataJson: null,
            dropWeight: 10,
            createdAt: createdAt);

        dbContext.Prizes.Add(prize);
        await dbContext.SaveChangesAsync();

    var repository = new PrizeReadRepository(dbContext);
    var handler = new GetPrizeQueryHandler(repository);

        var result = await handler.Handle(new GetPrizeQuery(prize.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(prize.Id);
        result.Name.Should().Be(prize.Name);
        result.DropWeight.Should().Be(prize.DropWeight);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenPrizeMissing()
    {
    await using var dbContext = DbContextFactory.CreateInMemory();
    var repository = new PrizeReadRepository(dbContext);
    var handler = new GetPrizeQueryHandler(repository);

        var result = await handler.Handle(new GetPrizeQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }
}
