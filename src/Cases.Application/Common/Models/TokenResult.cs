namespace Cases.Application.Common.Models;

public sealed record TokenResult(string Token, DateTimeOffset ExpiresAt);
