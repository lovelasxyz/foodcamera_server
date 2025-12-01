using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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

        // Parse user JSON if present (Telegram WebApp format)
        JsonElement? userJson = null;
        if (dictionary.TryGetValue("user", out var userJsonString) && !string.IsNullOrWhiteSpace(userJsonString))
        {
            try
            {
                userJson = JsonSerializer.Deserialize<JsonElement>(userJsonString);
            }
            catch (JsonException)
            {
                throw new UnauthorizedAccessException("Invalid user JSON in Telegram init data.");
            }
        }
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

        // Extract user data - either from parsed user JSON or flat dictionary (Login Widget format)
        string? telegramId = null;
        string? firstName = null;
        string? lastName = null;
        string? username = null;
        string? photoUrl = null;

        if (userJson.HasValue)
        {
            // WebApp format: user is a JSON object
            var user = userJson.Value;
            telegramId = user.TryGetProperty("id", out var idProp) ? idProp.ToString() : null;
            firstName = user.TryGetProperty("first_name", out var fnProp) ? fnProp.GetString() : null;
            lastName = user.TryGetProperty("last_name", out var lnProp) ? lnProp.GetString() : null;
            username = user.TryGetProperty("username", out var unProp) ? unProp.GetString() : null;
            photoUrl = user.TryGetProperty("photo_url", out var puProp) ? puProp.GetString() : null;
        }
        else
        {
            // Login Widget format: flat fields
            dictionary.TryGetValue("id", out telegramId);
            dictionary.TryGetValue("first_name", out firstName);
            dictionary.TryGetValue("last_name", out lastName);
            dictionary.TryGetValue("username", out username);
            dictionary.TryGetValue("photo_url", out photoUrl);
        }

        if (string.IsNullOrWhiteSpace(telegramId))
        {
            throw new UnauthorizedAccessException("Telegram user id is missing.");
        }

        if (!dictionary.TryGetValue("auth_date", out var authDateString) || !long.TryParse(authDateString, out var authDate))
        {
            throw new UnauthorizedAccessException("Telegram auth date is invalid.");
        }

        return new TelegramUserData(
            telegramId,
            string.IsNullOrWhiteSpace(firstName) ? null : firstName,
            string.IsNullOrWhiteSpace(lastName) ? null : lastName,
            string.IsNullOrWhiteSpace(username) ? null : username,
            string.IsNullOrWhiteSpace(photoUrl) ? null : photoUrl,
            authDate);
    }
}
