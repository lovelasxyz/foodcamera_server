using Cases.Application;
using Cases.Application.Common.Interfaces;
using System.Text;
using Cases.API.Middleware;
using Cases.API.Authorization;
using Cases.Infrastructure;
using Cases.Infrastructure.Authentication.Session;
using Cases.Infrastructure.Configuration;
using Cases.Infrastructure.RealTime;
using Cases.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddCors(options =>
{
    // Get allowed origins from configuration or use defaults
    var allowedOrigins = builder.Configuration
        .GetSection("CORS:AllowedOrigins")
        .Get<string[]>() ?? new[]
        {
            // Development origins
            "http://localhost:5173",
            "https://localhost:5173",
            "http://127.0.0.1:5173",
            "https://127.0.0.1:5173",
            "http://localhost:4173",
            "http://localhost:3000",
            "http://localhost:3001",
            // Production origin (update this to your Vercel URL)
            "https://cases-phi.vercel.app"
        };

    options.AddPolicy("AppClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var jwtSettings = builder.Configuration
    .GetSection("Authentication:Jwt")
    .Get<JwtSettings>() ?? throw new InvalidOperationException("JWT settings are not configured.");

if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
{
    throw new InvalidOperationException("JWT secret is not configured.");
}

var sessionCookieName = builder.Configuration.GetValue<string>("Authentication:Session:CookieName")
    ?? SessionAuthenticationDefaults.SessionCookieName;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Composite";
        options.DefaultChallengeScheme = "Composite";
    })
    .AddPolicyScheme("Composite", "SessionOrJwt", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return JwtBearerDefaults.AuthenticationScheme;
            }

            return context.Request.Cookies.ContainsKey(sessionCookieName)
                ? SessionAuthenticationDefaults.AuthenticationScheme
                : JwtBearerDefaults.AuthenticationScheme;
        };
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    })
    .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(
        SessionAuthenticationDefaults.AuthenticationScheme,
        _ => { })
    .AddScheme<AuthenticationSchemeOptions, BotApiKeyAuthenticationHandler>("BotApiKey", null);

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cases API",
        Version = "v1",
        Description = "Backend API for the Cases application"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cases API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors("AppClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<CasesHub>("/hubs/cases");

app.MapGet("/health", async (IUnitOfWork unitOfWork, CancellationToken cancellationToken) =>
{
    var databaseConnected = await unitOfWork.CanConnectAsync(cancellationToken);

    return Results.Ok(new
    {
        status = "Healthy",
        database = databaseConnected ? "Connected" : "Unavailable"
    });
});

app.Run();
