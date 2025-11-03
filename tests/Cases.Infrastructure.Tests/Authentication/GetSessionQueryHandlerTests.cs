using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces;
using Cases.Application.Identity.Queries.GetSession;
using Cases.Domain.Entities;
using Cases.Infrastructure.Persistence;
using Cases.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Cases.Infrastructure.Tests.Authentication;

public sealed class GetSessionQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsSessionData_WhenSessionExists()
    {
        await using var context = CreateDbContext();

        var now = new DateTimeOffset(2024, 1, 5, 10, 0, 0, TimeSpan.Zero);

        var user = User.CreateFromTelegram(
            telegramId: "555",
            firstName: "John",
            lastName: "Smith",
            username: "johnsmith",
            photoUrl: null,
            authDate: 1_700_000_500,
            now: now);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var expiresAt = now.AddHours(4);
        var session = UserSession.Create(user, now, expiresAt);
        context.UserSessions.Add(session);
        await context.SaveChangesAsync();

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.SetupGet(service => service.UserId).Returns(user.Id);
        currentUserService.SetupGet(service => service.SessionId).Returns(session.Id);

    var repository = new UserSessionReadRepository(context);
    var handler = new GetSessionQueryHandler(repository, currentUserService.Object);

        var result = await handler.Handle(new GetSessionQuery(), CancellationToken.None);

        result.SessionId.Should().Be(session.Id);
        result.SessionExpiresAt.Should().Be(expiresAt);
        result.User.Id.Should().Be(user.Id);
        result.User.TelegramUsername.Should().Be("johnsmith");
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorized_WhenSessionIsMissing()
    {
        await using var context = CreateDbContext();

        var now = new DateTimeOffset(2024, 1, 5, 10, 0, 0, TimeSpan.Zero);

        var user = User.CreateFromTelegram(
            telegramId: "555",
            firstName: "John",
            lastName: "Smith",
            username: "johnsmith",
            photoUrl: null,
            authDate: 1_700_000_500,
            now: now);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.SetupGet(service => service.UserId).Returns(user.Id);
        currentUserService.SetupGet(service => service.SessionId).Returns(Guid.NewGuid());

    var repository = new UserSessionReadRepository(context);
    var handler = new GetSessionQueryHandler(repository, currentUserService.Object);

        Func<Task> act = () => handler.Handle(new GetSessionQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    private static CasesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CasesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CasesDbContext(options);
    }
}
