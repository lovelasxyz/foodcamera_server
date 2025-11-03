using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Commands.PublishCase;
using Cases.Domain.Entities;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cases.Infrastructure.Tests.Cases.Commands;

public sealed class PublishCaseCommandHandlerTests
{
    [Fact]
    public async Task Handle_ActivatesCaseAndUpdatesVisibility()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var createdAt = new DateTimeOffset(2024, 03, 04, 8, 0, 0, TimeSpan.Zero);
        var initialCase = Case.Create(
            name: "Starter Case",
            image: null,
            price: 2,
            commissionPercent: 10,
            sortOrder: 0,
            autoHide: false,
            visibleFrom: createdAt,
            visibleUntil: null,
            createdAt: createdAt);

        dbContext.Cases.Add(initialCase);
        await dbContext.SaveChangesAsync();

        var publishFrom = createdAt.AddHours(2);
        var publishUntil = publishFrom.AddDays(1);
        var now = publishFrom.AddMinutes(-5);

        var repository = new CaseRepository(dbContext);
        var handler = new PublishCaseCommandHandler(repository, dbContext, new TestDateTimeProvider(now));

        await handler.Handle(new PublishCaseCommand(initialCase.Id, publishFrom, publishUntil), CancellationToken.None);

        var storedCase = await dbContext.Cases.SingleAsync();
        storedCase.IsActive.Should().BeTrue();
        storedCase.VisibleFrom.Should().Be(publishFrom);
        storedCase.VisibleUntil.Should().Be(publishUntil);
        storedCase.UpdatedAt.Should().Be(now);
    }

    [Fact]
    public async Task Handle_DefaultsVisibleFromToNow_WhenNotProvided()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var createdAt = new DateTimeOffset(2024, 03, 04, 8, 0, 0, TimeSpan.Zero);
        var initialCase = Case.Create(
            name: "Starter Case",
            image: null,
            price: 2,
            commissionPercent: 10,
            sortOrder: 0,
            autoHide: false,
            visibleFrom: createdAt,
            visibleUntil: null,
            createdAt: createdAt);

        dbContext.Cases.Add(initialCase);
        await dbContext.SaveChangesAsync();

        var now = createdAt.AddHours(3);
        var repository = new CaseRepository(dbContext);
        var handler = new PublishCaseCommandHandler(repository, dbContext, new TestDateTimeProvider(now));

        await handler.Handle(new PublishCaseCommand(initialCase.Id, null, null), CancellationToken.None);

        var storedCase = await dbContext.Cases.SingleAsync();
        storedCase.VisibleFrom.Should().Be(now);
        storedCase.VisibleUntil.Should().BeNull();
        storedCase.IsActive.Should().BeTrue();
    }
}
