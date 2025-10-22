using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Interfaces.Authentication;
using Cases.Application.Common.Models;
using Cases.Domain.Entities;
using Cases.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cases.Infrastructure.Authentication.Jwt;

public sealed class JwtTokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtTokenService(IOptions<JwtSettings> options, IDateTimeProvider dateTimeProvider)
    {
        _settings = options.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public TokenResult GenerateToken(User user)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role),
        };

        if (!string.IsNullOrWhiteSpace(user.Name))
        {
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
        }

        if (!string.IsNullOrWhiteSpace(user.TelegramId))
        {
            claims.Add(new Claim("telegram_id", user.TelegramId));
        }

        if (!string.IsNullOrWhiteSpace(user.TelegramUsername))
        {
            claims.Add(new Claim("telegram_username", user.TelegramUsername));
        }

        var now = _dateTimeProvider.UtcNow;
        var expiresAt = now.AddMinutes(_settings.ExpirationMinutes);

        var jwtToken = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return new TokenResult(token, expiresAt);
    }
}
