using System;
using Cases.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Cases.Queries.GetCases;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Domain.Entities;
using Cases.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Xunit;

namespace Cases.Infrastructure.Tests.Cases.Queries;

public sealed class GetCasesQueryHandlerTests
{
	[Fact]
	public async Task Handle_ReturnsOnlyActiveCases_WhenIncludeInactiveIsFalse()
	{
		await using var dbContext = DbContextFactory.CreateInMemory();
		var now = new DateTimeOffset(2024, 03, 07, 10, 0, 0, TimeSpan.Zero);

		var caseA = Case.Create("A", null, 1, 10, 2, false, now, null, now);
		caseA.SetActive(true, now);
		var caseB = Case.Create("B", null, 2, 10, 1, false, now, null, now);
		caseB.SetActive(false, now);
		var caseC = Case.Create("C", null, 3, 10, 0, false, now, null, now);
		caseC.SetActive(true, now);

		dbContext.Cases.AddRange(caseA, caseB, caseC);
		await dbContext.SaveChangesAsync();

	var repository = new CaseRepository(dbContext);
	var handler = new GetCasesQueryHandler(repository);

		var result = await handler.Handle(new GetCasesQuery(Limit: null, IncludeInactive: false), CancellationToken.None);

		result.Should().HaveCount(2);
		result[0].Name.Should().Be("C"); // sort order ascending among active
		result[1].Name.Should().Be("A");
	}

	[Fact]
	public async Task Handle_RespectsLimit_WhenProvided()
	{
		await using var dbContext = DbContextFactory.CreateInMemory();
		var now = new DateTimeOffset(2024, 03, 07, 10, 0, 0, TimeSpan.Zero);

		for (var i = 0; i < 5; i++)
		{
			var @case = Case.Create($"Case {i}", null, 1 + i, 10, i, false, now, null, now);
			@case.SetActive(true, now);
			dbContext.Cases.Add(@case);
		}

		await dbContext.SaveChangesAsync();

	var repository = new CaseRepository(dbContext);
	var handler = new GetCasesQueryHandler(repository);

		var result = await handler.Handle(new GetCasesQuery(Limit: 3, IncludeInactive: true), CancellationToken.None);

		result.Should().HaveCount(3);
		result.Should().BeInAscendingOrder(c => c.SortOrder);
	}
}
