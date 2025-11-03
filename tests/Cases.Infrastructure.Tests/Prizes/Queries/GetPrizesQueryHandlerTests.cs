using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Prizes.Queries.GetPrizes;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Domain.Entities;
using Cases.Domain.Enums;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Xunit;

namespace Cases.Infrastructure.Tests.Prizes.Queries;

public sealed class GetPrizesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyActivePrizes_WhenOnlyActiveTrue()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var now = new DateTimeOffset(2024, 03, 08, 9, 0, 0, TimeSpan.Zero);

    var activePrize = Prize.Create("Active", 1, null, PrizeRarity.Common, false, null, null, null, "active", false, false, false, null, null, 1, now);
    var inactivePrize = Prize.Create("Inactive", 1, null, PrizeRarity.Common, false, null, null, null, "inactive", false, false, false, null, null, 1, now);
        inactivePrize.Deactivate(now.AddHours(1));

        dbContext.Prizes.AddRange(activePrize, inactivePrize);
        await dbContext.SaveChangesAsync();

    var repository = new PrizeReadRepository(dbContext);
    var handler = new GetPrizesQueryHandler(repository);

        var result = await handler.Handle(new GetPrizesQuery(true), CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Name.Should().Be("Active");
    }

    [Fact]
    public async Task Handle_ReturnsAllPrizes_WhenOnlyActiveFalse()
    {
        await using var dbContext = DbContextFactory.CreateInMemory();
        var now = new DateTimeOffset(2024, 03, 08, 9, 0, 0, TimeSpan.Zero);

    var prizeA = Prize.Create("Active", 1, null, PrizeRarity.Common, false, null, null, null, "a", false, false, false, null, null, 1, now);
    var prizeB = Prize.Create("Inactive", 1, null, PrizeRarity.Common, false, null, null, null, "b", false, false, false, null, null, 1, now);
        prizeB.Deactivate(now.AddHours(1));

        dbContext.Prizes.AddRange(prizeA, prizeB);
        await dbContext.SaveChangesAsync();

    var repository = new PrizeReadRepository(dbContext);
    var handler = new GetPrizesQueryHandler(repository);

        var result = await handler.Handle(new GetPrizesQuery(false), CancellationToken.None);

        result.Should().HaveCount(2);
    }
}
