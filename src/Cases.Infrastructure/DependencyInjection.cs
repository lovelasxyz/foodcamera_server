using Cases.Application.Common.Interfaces;
using Cases.Application.Common.Interfaces.Authentication;
using Cases.Application.Cases.Interfaces;
using Cases.Application.Identity.Interfaces;
using Cases.Application.Prizes.Interfaces;
using Cases.Application.Users.Interfaces;
using Cases.Infrastructure.Authentication.Jwt;
using Cases.Infrastructure.Authentication.Session;
using Cases.Infrastructure.Authentication.Telegram;
using Cases.Infrastructure.Configuration;
using Cases.Infrastructure.Events;
using Cases.Infrastructure.Persistence;
using Cases.Infrastructure.Persistence.Extensions;
using Cases.Infrastructure.Persistence.Repositories;
using Cases.Infrastructure.RealTime;
using Cases.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

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

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapPostgresEnums();

        var dataSource = dataSourceBuilder.Build();
        services.AddSingleton<NpgsqlDataSource>(dataSource);

        services.AddDbContext<CasesDbContext>(options =>
        {
            options.UseNpgsql(dataSource, builder =>
            {
                builder.MigrationsAssembly(typeof(CasesDbContext).Assembly.FullName);
            });
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CasesDbContext>());
        services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();
        services.AddScoped<ICaseReadRepository, CaseRepository>();
        services.AddScoped<ICaseWriteRepository, CaseRepository>();
        services.AddScoped<IUserReadRepository, UserReadRepository>();
        services.AddScoped<IUserWriteRepository, UserWriteRepository>();
        services.AddScoped<IPrizeReadRepository, PrizeReadRepository>();
        services.AddScoped<IPrizeWriteRepository, PrizeWriteRepository>();
        services.AddScoped<IUserSessionReadRepository, UserSessionReadRepository>();
    services.AddScoped<IUserSessionWriteRepository, UserSessionWriteRepository>();

        services.Configure<JwtSettings>(configuration.GetSection("Authentication:Jwt"));
        services.Configure<TelegramSettings>(configuration.GetSection("Authentication:Telegram"));
        services.Configure<SessionSettings>(configuration.GetSection("Authentication:Session"));

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<ITelegramAuthService, TelegramAuthService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddScoped<IUserSessionService, UserSessionService>();
        services.AddSingleton<ICasesChangeNotifier, SignalRCasesChangeNotifier>();

        return services;
    }
}
