using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Commands.FreezeCase;
using Cases.Domain.Entities;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cases.Infrastructure.Tests.Cases.Commands;

public sealed class FreezeCaseCommandHandlerTests
{
    [Fact]
    public async Task Handle_DisablesCase()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var createdAt = new DateTimeOffset(2024, 03, 05, 9, 0, 0, TimeSpan.Zero);
        var caseEntity = Case.Create(
            name: "Starter Case",
            image: null,
            price: 2,
            commissionPercent: 10,
            sortOrder: 0,
            autoHide: false,
            visibleFrom: createdAt,
            visibleUntil: null,
            createdAt: createdAt);

        caseEntity.SetActive(true, createdAt.AddHours(1));

        dbContext.Cases.Add(caseEntity);
        await dbContext.SaveChangesAsync();

        var now = createdAt.AddHours(2);
        var repository = new CaseRepository(dbContext);
        var handler = new FreezeCaseCommandHandler(repository, dbContext, new TestDateTimeProvider(now));

        await handler.Handle(new FreezeCaseCommand(caseEntity.Id), CancellationToken.None);

        var storedCase = await dbContext.Cases.SingleAsync();
        storedCase.IsActive.Should().BeFalse();
        storedCase.UpdatedAt.Should().Be(now);
    }
}
