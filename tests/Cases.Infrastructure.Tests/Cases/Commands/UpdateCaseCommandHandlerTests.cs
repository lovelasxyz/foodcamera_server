using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Commands.UpdateCase;
using Cases.Domain.Entities;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cases.Infrastructure.Tests.Cases.Commands;

public sealed class UpdateCaseCommandHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesCaseDetails()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var createdAt = new DateTimeOffset(2024, 03, 08, 9, 0, 0, TimeSpan.Zero);
        var caseEntity = Case.Create(
            name: "Starter",
            image: "image",
            price: 2,
            commissionPercent: 10,
            sortOrder: 1,
            autoHide: false,
            visibleFrom: createdAt,
            visibleUntil: null,
            createdAt: createdAt);

        dbContext.Cases.Add(caseEntity);
        await dbContext.SaveChangesAsync();

        var now = createdAt.AddHours(1);
        var repository = new CaseRepository(dbContext);
        var handler = new UpdateCaseCommandHandler(repository, dbContext, new TestDateTimeProvider(now));

        var command = new UpdateCaseCommand(
            caseEntity.Id,
            NameSpecified: true,
            Name: "Updated",
            ImageSpecified: true,
            Image: "new.png",
            PriceSpecified: true,
            Price: 5m,
            CommissionPercentSpecified: true,
            CommissionPercent: 25m,
            SortOrderSpecified: true,
            SortOrder: 3,
            AutoHideSpecified: true,
            AutoHide: true,
            VisibleFromSpecified: true,
            VisibleFrom: now.AddHours(1),
            VisibleUntilSpecified: true,
            VisibleUntil: now.AddDays(1));

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Cases.SingleAsync();
        stored.Name.Should().Be("Updated");
        stored.Image.Should().Be("new.png");
        stored.Price.Should().Be(5m);
        stored.CommissionPercent.Should().Be(25m);
        stored.SortOrder.Should().Be(3);
        stored.AutoHide.Should().BeTrue();
        stored.VisibleFrom.Should().Be(now.AddHours(1));
        stored.VisibleUntil.Should().Be(now.AddDays(1));
        stored.UpdatedAt.Should().Be(now);
    }

    [Fact]
    public async Task Handle_PreservesMissingFields_WhenNotProvided()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var createdAt = new DateTimeOffset(2024, 03, 08, 9, 0, 0, TimeSpan.Zero);
        var caseEntity = Case.Create(
            name: "Starter",
            image: "image",
            price: 2,
            commissionPercent: 10,
            sortOrder: 1,
            autoHide: false,
            visibleFrom: createdAt,
            visibleUntil: null,
            createdAt: createdAt);

        dbContext.Cases.Add(caseEntity);
        await dbContext.SaveChangesAsync();

        var now = createdAt.AddHours(1);
        var repository = new CaseRepository(dbContext);
        var handler = new UpdateCaseCommandHandler(repository, dbContext, new TestDateTimeProvider(now));

        var command = new UpdateCaseCommand(
            caseEntity.Id,
            NameSpecified: false,
            Name: null,
            ImageSpecified: true,
            Image: "updated.png",
            PriceSpecified: false,
            Price: null,
            CommissionPercentSpecified: false,
            CommissionPercent: null,
            SortOrderSpecified: false,
            SortOrder: null,
            AutoHideSpecified: false,
            AutoHide: null,
            VisibleFromSpecified: false,
            VisibleFrom: null,
            VisibleUntilSpecified: false,
            VisibleUntil: null);

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Cases.SingleAsync();
        stored.Name.Should().Be("Starter");
        stored.Image.Should().Be("updated.png");
        stored.Price.Should().Be(2);
        stored.CommissionPercent.Should().Be(10);
        stored.SortOrder.Should().Be(1);
        stored.AutoHide.Should().BeFalse();
        stored.VisibleFrom.Should().Be(createdAt);
        stored.VisibleUntil.Should().BeNull();
    }
}
