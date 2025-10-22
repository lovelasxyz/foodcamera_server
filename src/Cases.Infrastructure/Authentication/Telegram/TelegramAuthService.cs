using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Cases.Application.Common.Interfaces.Authentication;
using Cases.Application.Common.Models;
using Cases.Infrastructure.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Cases.Infrastructure.Authentication.Telegram;

public sealed class TelegramAuthService : ITelegramAuthService
{
    private readonly TelegramSettings _settings;

    public TelegramAuthService(IOptions<TelegramSettings> options)
    {
        _settings = options.Value;
    }

    public TelegramUserData ValidateAndParse(string initData)
    {
        if (string.IsNullOrWhiteSpace(_settings.BotToken))
        {
            throw new InvalidOperationException("Telegram bot token is not configured.");
        }

        if (string.IsNullOrWhiteSpace(initData))
        {
            throw new UnauthorizedAccessException("Telegram init data is missing.");
        }

        var parsed = QueryHelpers.ParseQuery(initData);
        var dictionary = parsed
            .Where(pair => pair.Key is not null)
            .ToDictionary(
                pair => pair.Key!,
                pair => pair.Value.ToString(),
                StringComparer.Ordinal);

        if (!dictionary.TryGetValue("hash", out var providedHash))
        {
            throw new UnauthorizedAccessException("Telegram hash is missing.");
        }

        var dataCheckString = string.Join(
            "\n",
            dictionary
                .Where(pair => !string.Equals(pair.Key, "hash", StringComparison.Ordinal))
                .OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .Select(pair => $"{pair.Key}={pair.Value}"));

        var secretKey = SHA256.HashData(Encoding.UTF8.GetBytes(_settings.BotToken));

        using var hmac = new HMACSHA256(secretKey);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));
        var computedHashHex = Convert.ToHexString(computedHash).ToLowerInvariant();

        if (!string.Equals(computedHashHex, providedHash, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Telegram auth data signature is invalid.");
        }

        if (!dictionary.TryGetValue("id", out var telegramId))
        {
            throw new UnauthorizedAccessException("Telegram user id is missing.");
        }

        if (!dictionary.TryGetValue("auth_date", out var authDateString) || !long.TryParse(authDateString, out var authDate))
        {
            throw new UnauthorizedAccessException("Telegram auth date is invalid.");
        }

        dictionary.TryGetValue("first_name", out var firstName);
        dictionary.TryGetValue("last_name", out var lastName);
        dictionary.TryGetValue("username", out var username);
        dictionary.TryGetValue("photo_url", out var photoUrl);

        return new TelegramUserData(
            telegramId,
            string.IsNullOrWhiteSpace(firstName) ? null : firstName,
            string.IsNullOrWhiteSpace(lastName) ? null : lastName,
            string.IsNullOrWhiteSpace(username) ? null : username,
            string.IsNullOrWhiteSpace(photoUrl) ? null : photoUrl,
            authDate);
    }
}
