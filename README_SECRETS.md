# Configuration Secrets

Для разработки используйте User Secrets или переменные окружения.

## User Secrets (Рекомендуется для разработки)

```bash
dotnet user-secrets init --project src/Cases.API
dotnet user-secrets set "ConnectionStrings:Database" "Host=localhost;Port=5433;Database=case_db;Username=postgres;Password=postgres" --project src/Cases.API
dotnet user-secrets set "Authentication:Jwt:Secret" "YOUR_JWT_SECRET_HERE" --project src/Cases.API
dotnet user-secrets set "Authentication:Telegram:BotToken" "YOUR_TELEGRAM_BOT_TOKEN" --project src/Cases.API
dotnet user-secrets set "BotApiKey" "YOUR_BOT_API_KEY" --project src/Cases.API
```

## Environment Variables (Production)

```bash
export ConnectionStrings__Database="YOUR_PRODUCTION_CONNECTION_STRING"
export Authentication__Jwt__Secret="YOUR_PRODUCTION_JWT_SECRET"
export Authentication__Telegram__BotToken="YOUR_TELEGRAM_BOT_TOKEN"
export BotApiKey="YOUR_PRODUCTION_BOT_API_KEY"
```

## Docker Environment Variables

```yaml
environment:
  - ConnectionStrings__Database=Host=db;Database=case_db;Username=postgres;Password=${DB_PASSWORD}
  - Authentication__Jwt__Secret=${JWT_SECRET}
  - Authentication__Telegram__BotToken=${TELEGRAM_BOT_TOKEN}
  - BotApiKey=${BOT_API_KEY}
```

**ВАЖНО:** Никогда не коммитьте реальные секреты в git!
