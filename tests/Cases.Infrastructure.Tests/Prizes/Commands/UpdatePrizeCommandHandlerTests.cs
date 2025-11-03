using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Prizes.Commands.UpdatePrize;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using Cases.Domain.Exceptions;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cases.Infrastructure.Tests.Prizes.Commands;

public sealed class UpdatePrizeCommandHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesPrizeFields()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var createdAt = new DateTimeOffset(2024, 03, 08, 12, 0, 0, TimeSpan.Zero);
        var prize = Prize.Create(
            name: "Gem",
            price: 1,
            image: null,
            rarity: PrizeRarity.Common,
            isShard: false,
            shardKey: null,
            shardsRequired: null,
            description: "original",
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

    var now = createdAt.AddHours(1);
    var repository = new PrizeWriteRepository(dbContext);
    var handler = new UpdatePrizeCommandHandler(repository, dbContext, new TestDateTimeProvider(now));

        var command = new UpdatePrizeCommand(
            prize.Id,
            Name: "Mega Gem",
            Price: 5,
            Image: "image.png",
            Rarity: "rare",
            IsShard: true,
            ShardKey: "mega",
            ShardsRequired: 10,
            Description: "updated",
            UniqueKey: "gem-1",
            Stackable: true,
            NotAwardIfOwned: true,
            NonRemovableGift: true,
            BenefitType: "discount",
            BenefitDataJson: "{\"percent\":50}",
            DropWeight: 3,
            IsActive: false);

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Prizes.SingleAsync();
        stored.Name.Should().Be("Mega Gem");
        stored.Price.Should().Be(5);
        stored.Rarity.Should().Be(PrizeRarity.Rare);
        stored.IsShard.Should().BeTrue();
        stored.Stackable.Should().BeTrue();
        stored.NotAwardIfOwned.Should().BeTrue();
        stored.NonRemovableGift.Should().BeTrue();
        stored.BenefitType.Should().Be(BenefitType.Discount);
        stored.DropWeight.Should().Be(3);
        stored.IsActive.Should().BeFalse();
        stored.UpdatedAt.Should().Be(now);
    }

    [Fact]
    public async Task Handle_Throws_WhenUniqueKeyUsedByAnotherPrize()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();

        var existing = Prize.Create(
            name: "Existing",
            price: 1,
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
            dropWeight: 10,
            createdAt: DateTimeOffset.UtcNow);

        var toUpdate = Prize.Create(
            name: "Update",
            price: 2,
            image: null,
            rarity: PrizeRarity.Common,
            isShard: false,
            shardKey: null,
            shardsRequired: null,
            description: null,
            uniqueKey: "original",
            stackable: false,
            notAwardIfOwned: false,
            nonRemovableGift: false,
            benefitType: null,
            benefitDataJson: null,
            dropWeight: 5,
            createdAt: DateTimeOffset.UtcNow);

        dbContext.Prizes.AddRange(existing, toUpdate);
        await dbContext.SaveChangesAsync();

    var repository = new PrizeWriteRepository(dbContext);
    var handler = new UpdatePrizeCommandHandler(repository, dbContext, new TestDateTimeProvider(DateTimeOffset.UtcNow));

        var command = new UpdatePrizeCommand(
            toUpdate.Id,
            Name: toUpdate.Name,
            Price: toUpdate.Price,
            Image: toUpdate.Image,
            Rarity: toUpdate.Rarity.ToDatabaseValue(),
            IsShard: toUpdate.IsShard,
            ShardKey: toUpdate.ShardKey,
            ShardsRequired: toUpdate.ShardsRequired,
            Description: toUpdate.Description,
            UniqueKey: "duplicate",
            Stackable: toUpdate.Stackable,
            NotAwardIfOwned: toUpdate.NotAwardIfOwned,
            NonRemovableGift: toUpdate.NonRemovableGift,
            BenefitType: toUpdate.BenefitType.ToDatabaseValue(),
            BenefitDataJson: toUpdate.BenefitDataJson,
            DropWeight: toUpdate.DropWeight,
            IsActive: toUpdate.IsActive);

        await FluentActions.Invoking(() => handler.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<DuplicateException>()
            .WithMessage("Prize with unique key 'duplicate' already exists.");
    }
}
