using System;
using System.Threading;
using System.Threading.Tasks;
using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Interfaces.Authentication;
using Cases.Application.Common.Models;
using Cases.Application.Identity.Commands.AuthenticateTelegram;
using Cases.Domain.Entities;
using Cases.Infrastructure.Persistence;
using Cases.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Cases.Infrastructure.Tests.Authentication;

public sealed class AuthenticateTelegramCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesUserAndReturnsToken_WhenUserDoesNotExist()
    {
        await using var context = CreateDbContext();

        var telegramUser = new TelegramUserData(
            Id: "777", 
            FirstName: "Jane", 
            LastName: "Doe", 
            Username: "janed", 
            PhotoUrl: "https://cdn.example.com/jane.png", 
            AuthDate: 1_700_000_000);

        var telegramAuthService = new Mock<ITelegramAuthService>();
        telegramAuthService
            .Setup(service => service.ValidateAndParse(It.IsAny<string>()))
            .Returns(telegramUser);

        var tokenResult = new TokenResult("token-123", DateTimeOffset.UtcNow.AddHours(1));

        var tokenService = new Mock<ITokenService>();
        tokenService
            .Setup(service => service.GenerateToken(It.IsAny<User>()))
            .Returns(tokenResult);

        var now = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider
            .Setup(provider => provider.UtcNow)
            .Returns(now);

        var expectedSessionExpiresAt = now.AddDays(30);
        UserSession? capturedSession = null;

        var sessionService = new Mock<IUserSessionService>();
        sessionService
            .Setup(service => service.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User createdUser, CancellationToken _) =>
            {
                capturedSession = UserSession.Create(createdUser, now, expectedSessionExpiresAt);
                return capturedSession;
            });

        var userRepository = new UserWriteRepository(context);

        var handler = new AuthenticateTelegramCommandHandler(
            userRepository,
            telegramAuthService.Object,
            tokenService.Object,
            dateTimeProvider.Object,
            sessionService.Object,
            context);

        var command = new AuthenticateTelegramCommand("init-data");

        var result = await handler.Handle(command, CancellationToken.None);

    result.AccessToken.Should().Be(tokenResult.Token);
    result.TokenExpiresAt.Should().Be(tokenResult.ExpiresAt);
    result.SessionExpiresAt.Should().Be(expectedSessionExpiresAt);
    result.SessionId.Should().Be(capturedSession!.Id);
        result.User.Name.Should().Be("Jane Doe");
        result.User.TelegramUsername.Should().Be("janed");

        var user = await context.Users.SingleAsync();
        user.TelegramId.Should().Be("777");
        user.Name.Should().Be("Jane Doe");
        user.CreatedAt.Should().Be(now);
        user.UpdatedAt.Should().Be(now);
        user.TelegramHasPhoto.Should().BeTrue();
        user.TelegramPhotoUrl.Should().Be(telegramUser.PhotoUrl);
        user.LastAuthAt.Should().Be(telegramUser.AuthDate);

        tokenService.Verify(service => service.GenerateToken(user), Times.Once);
    sessionService.Verify(service => service.CreateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdatesExistingUser_WhenUserAlreadyExists()
    {
        await using var context = CreateDbContext();

        var originalNow = new DateTimeOffset(2023, 12, 31, 23, 0, 0, TimeSpan.Zero);
        var existingUser = User.CreateFromTelegram(
            telegramId: "777",
            firstName: "Jane",
            lastName: "D",
            username: "jane_old",
            photoUrl: null,
            authDate: 1_600_000_000,
            now: originalNow);

        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var telegramUser = new TelegramUserData(
            Id: "777",
            FirstName: "Jane",
            LastName: "Doe",
            Username: "janed",
            PhotoUrl: "https://cdn.example.com/jane.png",
            AuthDate: 1_700_000_000);

        var telegramAuthService = new Mock<ITelegramAuthService>();
        telegramAuthService
            .Setup(service => service.ValidateAndParse(It.IsAny<string>()))
            .Returns(telegramUser);

        var tokenResult = new TokenResult("token-456", DateTimeOffset.UtcNow.AddHours(2));

        var tokenService = new Mock<ITokenService>();
        tokenService
            .Setup(service => service.GenerateToken(It.IsAny<User>()))
            .Returns(tokenResult);

        var updatedNow = new DateTimeOffset(2024, 1, 2, 10, 0, 0, TimeSpan.Zero);

        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider
            .Setup(provider => provider.UtcNow)
            .Returns(updatedNow);

        var expectedSessionExpiresAt = updatedNow.AddDays(30);
        UserSession? capturedSession = null;

        var sessionService = new Mock<IUserSessionService>();
        sessionService
            .Setup(service => service.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User updatedUser, CancellationToken _) =>
            {
                capturedSession = UserSession.Create(updatedUser, updatedNow, expectedSessionExpiresAt);
                return capturedSession;
            });

        var userRepository = new UserWriteRepository(context);

        var handler = new AuthenticateTelegramCommandHandler(
            userRepository,
            telegramAuthService.Object,
            tokenService.Object,
            dateTimeProvider.Object,
            sessionService.Object,
            context);

        var command = new AuthenticateTelegramCommand("init-data");

        var result = await handler.Handle(command, CancellationToken.None);

        result.User.TelegramUsername.Should().Be("janed");
        result.User.Name.Should().Be("Jane Doe");
    result.SessionId.Should().Be(capturedSession!.Id);
    result.SessionExpiresAt.Should().Be(expectedSessionExpiresAt);

        var user = await context.Users.SingleAsync();
        user.Id.Should().Be(existingUser.Id);
        user.TelegramUsername.Should().Be("janed");
        user.TelegramHasPhoto.Should().BeTrue();
        user.TelegramPhotoUrl.Should().Be(telegramUser.PhotoUrl);
        user.LastAuthAt.Should().Be(telegramUser.AuthDate);
        user.TelegramRegisteredAt.Should().Be(1_600_000_000);
        user.UpdatedAt.Should().Be(updatedNow);
        user.CreatedAt.Should().Be(originalNow);

        tokenService.Verify(service => service.GenerateToken(user), Times.Once);
    sessionService.Verify(service => service.CreateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static CasesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CasesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CasesDbContext(options);
    }
}
