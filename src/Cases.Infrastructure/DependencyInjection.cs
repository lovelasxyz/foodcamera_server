using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Interfaces.Authentication;
using Cases.Infrastructure.Authentication.Jwt;
using Cases.Infrastructure.Authentication.Telegram;
using Cases.Infrastructure.Configuration;
using Cases.Infrastructure.Persistence;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cases.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'Database' is missing in configuration.");
        }

        services.AddDbContext<CasesDbContext>(options =>
        {
            options.UseNpgsql(connectionString,
                builder => builder.MigrationsAssembly(typeof(CasesDbContext).Assembly.FullName));
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<CasesDbContext>());
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        services.Configure<JwtSettings>(configuration.GetSection("Authentication:Jwt"));
        services.Configure<TelegramSettings>(configuration.GetSection("Authentication:Telegram"));

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<ITelegramAuthService, TelegramAuthService>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
