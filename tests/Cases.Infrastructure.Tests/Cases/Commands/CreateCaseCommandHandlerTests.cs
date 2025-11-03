using System;
using Cases.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Commands.CreateCase;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cases.Infrastructure.Tests.Cases.Commands;

public sealed class CreateCaseCommandHandlerTests
{
    [Fact]
    public async Task Handle_PersistsCaseWithDefaults_WhenRequestValid()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var now = new DateTimeOffset(2024, 03, 01, 9, 0, 0, TimeSpan.Zero);
    var dateProvider = new TestDateTimeProvider(now);
    var repository = new CaseRepository(dbContext);
    var handler = new CreateCaseCommandHandler(repository, dbContext, dateProvider);

        var command = new CreateCaseCommand(
            Name: "Starter Case",
            Image: "https://cdn.example.com/case.png",
            Price: 2.5m,
            CommissionPercent: 15,
            SortOrder: 1,
            AutoHide: false,
            VisibleFrom: null,
            VisibleUntil: null);

        var caseId = await handler.Handle(command, CancellationToken.None);

        caseId.Should().BeGreaterThan(0);

        var storedCase = await dbContext.Cases.SingleAsync();
        storedCase.Id.Should().Be(caseId);
        storedCase.Name.Should().Be(command.Name);
        storedCase.IsActive.Should().BeFalse();
        storedCase.Price.Should().Be(command.Price);
        storedCase.VisibleFrom.Should().Be(now);
        storedCase.VisibleUntil.Should().BeNull();
        storedCase.CreatedAt.Should().Be(now);
        storedCase.UpdatedAt.Should().Be(now);
        storedCase.CasePrizes.Should().BeEmpty();
    }
}
