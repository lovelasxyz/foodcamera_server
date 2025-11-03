using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Prizes.Commands.CreatePrize;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using Cases.Domain.Exceptions;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cases.Infrastructure.Tests.Prizes.Commands;

public sealed class CreatePrizeCommandHandlerTests
{
    [Fact]
    public async Task Handle_PersistsPrize_WhenRequestValid()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
    var now = new DateTimeOffset(2024, 02, 01, 12, 0, 0, TimeSpan.Zero);
    var dateTimeProvider = new TestDateTimeProvider(now);
    var repository = new PrizeWriteRepository(dbContext);
    var handler = new CreatePrizeCommandHandler(repository, dbContext, dateTimeProvider);

        var command = new CreatePrizeCommand(
            Name: "Super Prize",
            Price: 9.99m,
            Image: "https://cdn.example.com/prize.png",
            Rarity: "rare",
            IsShard: false,
            ShardKey: null,
            ShardsRequired: null,
            Description: "A rare reward",
            UniqueKey: "super-prize",
            Stackable: false,
            NotAwardIfOwned: false,
            NonRemovableGift: false,
            BenefitType: null,
            BenefitDataJson: null,
            DropWeight: 5.5m);

        var prizeId = await handler.Handle(command, CancellationToken.None);

        prizeId.Should().BeGreaterThan(0);

        var storedPrize = await dbContext.Prizes.SingleAsync();
        storedPrize.Id.Should().Be(prizeId);
        storedPrize.Name.Should().Be(command.Name);
        storedPrize.Price.Should().Be(command.Price);
        storedPrize.Rarity.Should().Be(PrizeRarity.Rare);
        storedPrize.UniqueKey.Should().Be(command.UniqueKey);
        storedPrize.IsActive.Should().BeTrue();
        storedPrize.CreatedAt.Should().Be(now);
        storedPrize.UpdatedAt.Should().Be(now);
    }

    [Fact]
    public async Task Handle_Throws_WhenUniqueKeyAlreadyExists()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();

        var existingPrize = Prize.Create(
            name: "Existing",
            price: 5,
            image: null,
            rarity: PrizeRarity.Common,
            isShard: false,
            shardKey: null,
            shardsRequired: null,
            description: null,
            uniqueKey: "duplicate",
            stackable: false,
            notAwardIfOwned: false,
            nonRemovableGift: false,
            benefitType: null,
            benefitDataJson: null,
            dropWeight: 1,
            createdAt: new DateTimeOffset(2023, 12, 31, 12, 0, 0, TimeSpan.Zero));

        dbContext.Prizes.Add(existingPrize);
        await dbContext.SaveChangesAsync();

    var repository = new PrizeWriteRepository(dbContext);
    var handler = new CreatePrizeCommandHandler(repository, dbContext, new TestDateTimeProvider(DateTimeOffset.UtcNow));

        var command = new CreatePrizeCommand(
            Name: "Another",
            Price: 10,
            Image: null,
            Rarity: "rare",
            IsShard: false,
            ShardKey: null,
            ShardsRequired: null,
            Description: null,
            UniqueKey: "duplicate",
            Stackable: false,
            NotAwardIfOwned: false,
            NonRemovableGift: false,
            BenefitType: null,
            BenefitDataJson: null,
            DropWeight: 2);

        await FluentActions.Invoking(() => handler.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<DuplicateException>()
            .WithMessage("Prize with unique key 'duplicate' already exists.");
    }
}
