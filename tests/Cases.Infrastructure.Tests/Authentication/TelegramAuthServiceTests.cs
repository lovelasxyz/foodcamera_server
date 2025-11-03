using System;
using Cases.Domain.Exceptions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Cases.Infrastructure.Authentication.Telegram;
using Cases.Infrastructure.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Cases.Infrastructure.Tests.Authentication;

public sealed class TelegramAuthServiceTests
{
    private const string BotToken = "test-bot-token";

    [Fact]
    public void ValidateAndParse_ReturnsTelegramUserData_WhenInitDataIsValid()
    {
        var service = CreateService(BotToken);
        var unixAuthDate = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero).ToUnixTimeSeconds();

        var initData = TelegramInitDataBuilder
            .Create(BotToken)
            .WithId("123456")
            .WithFirstName("Ada")
            .WithLastName("Lovelace")
            .WithUsername("ada_bot")
            .WithPhotoUrl("https://cdn.example.com/photo.png")
            .WithAuthDate(unixAuthDate)
            .Build();

        var result = service.ValidateAndParse(initData);

        result.Id.Should().Be("123456");
        result.FirstName.Should().Be("Ada");
        result.LastName.Should().Be("Lovelace");
        result.Username.Should().Be("ada_bot");
        result.PhotoUrl.Should().Be("https://cdn.example.com/photo.png");
        result.AuthDate.Should().Be(unixAuthDate);
    }

    [Fact]
    public void ValidateAndParse_ThrowsUnauthorized_WhenHashDoesNotMatch()
    {
        var service = CreateService(BotToken);
        var initData = TelegramInitDataBuilder
            .Create(BotToken)
            .WithId("123456")
            .WithAuthDate(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds())
            .Build();

        var tampered = initData.Replace("hash=", "hash=deadbeef", StringComparison.Ordinal);

        var act = () => service.ValidateAndParse(tampered);

        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("Telegram auth data signature is invalid.");
    }

    [Fact]
    public void ValidateAndParse_ThrowsUnauthorized_WhenInitDataIsEmpty()
    {
        var service = CreateService(BotToken);

        var act = () => service.ValidateAndParse(string.Empty);

        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("Telegram init data is missing.");
    }

    [Fact]
    public void ValidateAndParse_ThrowsInvalidOperation_WhenBotTokenMissing()
    {
        var service = CreateService(string.Empty);

        var act = () => service.ValidateAndParse("id=1&hash=abc");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Telegram bot token is not configured.");
    }

    private static TelegramAuthService CreateService(string botToken)
    {
        return new TelegramAuthService(Options.Create(new TelegramSettings
        {
            BotToken = botToken
        }));
    }

    private sealed class TelegramInitDataBuilder
    {
        private readonly string _botToken;
        private readonly Dictionary<string, string> _payload = new(StringComparer.Ordinal);

        private TelegramInitDataBuilder(string botToken)
        {
            _botToken = botToken;
        }

        public static TelegramInitDataBuilder Create(string botToken)
        {
            return new TelegramInitDataBuilder(botToken);
        }

        public TelegramInitDataBuilder WithId(string id)
        {
            _payload["id"] = id;
            return this;
        }

        public TelegramInitDataBuilder WithFirstName(string? value)
        {
            return SetOptional("first_name", value);
        }

        public TelegramInitDataBuilder WithLastName(string? value)
        {
            return SetOptional("last_name", value);
        }

        public TelegramInitDataBuilder WithUsername(string? value)
        {
            return SetOptional("username", value);
        }

        public TelegramInitDataBuilder WithPhotoUrl(string? value)
        {
            return SetOptional("photo_url", value);
        }

        public TelegramInitDataBuilder WithAuthDate(long authDate)
        {
            _payload["auth_date"] = authDate.ToString(CultureInfo.InvariantCulture);
            return this;
        }

        public string Build()
        {
            if (!_payload.ContainsKey("id"))
            {
                throw new InvalidOperationException("Telegram init data builder requires an id.");
            }

            if (!_payload.ContainsKey("auth_date"))
            {
                throw new InvalidOperationException("Telegram init data builder requires an auth_date.");
            }

            var dataCheckString = string.Join(
                '\n',
                _payload
                    .OrderBy(pair => pair.Key, StringComparer.Ordinal)
                    .Select(pair => $"{pair.Key}={pair.Value}"));

            var secretKey = SHA256.HashData(Encoding.UTF8.GetBytes(_botToken));
            using var hmac = new HMACSHA256(secretKey);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));
            var hashHex = Convert.ToHexString(computedHash).ToLowerInvariant();

            var parameters = _payload
                .Select(pair => $"{pair.Key}={Uri.EscapeDataString(pair.Value)}")
                .Append($"hash={hashHex}");

            return string.Join('&', parameters);
        }

        private TelegramInitDataBuilder SetOptional(string key, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _payload[key] = value;
            }
            else
            {
                _payload.Remove(key);
            }

            return this;
        }
    }
}
