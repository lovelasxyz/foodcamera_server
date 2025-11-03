namespace Cases.Infrastructure.Configuration;

public sealed class SessionSettings
{
    public string CookieName { get; set; } = "session";
    public int LifetimeMinutes { get; set; } = 60 * 24 * 30; // 30 days
    public bool SlidingExpiration { get; set; } = true;
    public bool Secure { get; set; } = true;
    public string SameSite { get; set; } = "Lax";
    public string Path { get; set; } = "/";
    public string? Domain { get; set; }
    public bool HttpOnly { get; set; } = true;
}