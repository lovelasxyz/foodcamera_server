using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces;
using Cases.Domain.Common;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using Cases.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Cases.Infrastructure.Tests.Infrastructure;

public sealed class CasesDbContextDomainEventTests
{
    [Fact]
    public async Task SaveChangesAsync_DispatchesAndClearsDomainEvents()
    {
        var options = new DbContextOptionsBuilder<CasesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dispatcher = new Mock<IDomainEventDispatcher>();

        await using var context = new CasesDbContext(options, dispatcher.Object);

        var prize = Prize.Create(
            name: "Test",
            price: 10m,
            image: null,
            rarity: PrizeRarity.Common,
            isShard: false,
            shardKey: null,
            shardsRequired: null,
            description: null,
            uniqueKey: null,
            stackable: false,
            notAwardIfOwned: false,
            nonRemovableGift: false,
            benefitType: null,
            benefitDataJson: null,
            dropWeight: 1m,
            createdAt: DateTimeOffset.UtcNow);

        var domainEvent = new TestDomainEvent(DateTimeOffset.UtcNow);
        var raiseMethod = typeof(Prize).GetMethod("RaiseDomainEvent", BindingFlags.Instance | BindingFlags.NonPublic);
        raiseMethod!.Invoke(prize, new object[] { domainEvent });

        context.Prizes.Add(prize);
        await context.SaveChangesAsync();

        dispatcher.Verify(
            d => d.DispatchAsync(
                It.Is<IEnumerable<IDomainEvent>>(events => events.Contains(domainEvent)),
                It.IsAny<CancellationToken>()),
            Times.Once);

        prize.DomainEvents.Should().BeEmpty();
    }

    private sealed record TestDomainEvent(DateTimeOffset OccurredOn) : IDomainEvent;
}