# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**net-garminconnect** - .NET 8+ бібліотека для взаємодії з Garmin Connect API.

Це C# порт Python бібліотеки [python-garminconnect](https://github.com/cyberjunky/python-garminconnect).

## Project Status

**Поточна фаза:** Фаза 7 - Тестування та документація

**Завершено:**
- ✅ Фаза 1: Базова інфраструктура (solution, projects, exceptions, HTTP client, endpoints)
- ✅ Фаза 2: Автентифікація (OAuth flow, Token Store, MFA)
- ✅ Фаза 3: Основні API (Health & Activities)
- ✅ Фаза 4: FIT протокол (Weight, Blood Pressure)
- ✅ Фаза 5: Розширені API (Body Composition, Devices, Gear, Workouts, Badges, Goals, Training Plans)
- ✅ Фаза 6: DI та Resilience (ServiceCollectionExtensions, Retry/Circuit Breaker policies)

**Наступні:**
- 📋 Фаза 7: Тестування та документація

**Тести:** 197 unit tests (всі проходять)

## Project Structure

```
net-garminconnect/
├── GarminConnect.sln
├── src/GarminConnect/
│   ├── Api/
│   │   ├── Endpoints.cs           # URL endpoints для Garmin API
│   │   ├── IGarminApiClient.cs    # Інтерфейс HTTP клієнта
│   │   └── GarminApiClient.cs     # Реалізація HTTP клієнта
│   ├── Auth/
│   │   ├── IGarminAuthenticator.cs  # Інтерфейс автентифікації
│   │   ├── GarminAuthenticator.cs   # Реалізація автентифікації
│   │   ├── GarminSsoClient.cs       # SSO клієнт
│   │   ├── AuthResult.cs            # Результат автентифікації
│   │   ├── OAuth/                   # OAuth токени та storage
│   │   └── Mfa/                     # MFA handlers
│   ├── Exceptions/                  # Ієрархія виключень
│   ├── Extensions/
│   │   └── ServiceCollectionExtensions.cs  # DI extensions
│   ├── Fit/                         # FIT протокол кодування
│   ├── Models/                      # DTO records
│   ├── GarminClientOptions.cs       # Конфігурація клієнта
│   ├── IGarminClient.cs             # Головний інтерфейс
│   ├── GarminClient.cs              # Головний клієнт
│   ├── GarminClient.Activity.cs     # Activities API
│   ├── GarminClient.Health.cs       # Health API
│   ├── GarminClient.User.cs         # User API
│   ├── GarminClient.BodyComposition.cs  # Body Composition API
│   ├── GarminClient.Devices.cs      # Devices API
│   ├── GarminClient.Gear.cs         # Gear API
│   ├── GarminClient.Workouts.cs     # Workouts API
│   ├── GarminClient.Badges.cs       # Badges API
│   ├── GarminClient.Goals.cs        # Goals API
│   └── GarminClient.TrainingPlans.cs # Training Plans API
└── tests/GarminConnect.Tests/       # Unit tests
```

## Common Commands

```bash
# Build
dotnet build

# Run tests
dotnet test

# Run single test
dotnet test --filter "FullyQualifiedName~TestMethodName"

# Pack NuGet package
dotnet pack -c Release
```

## Architecture Decisions

| Аспект | Рішення |
|--------|---------|
| Target Framework | .NET 8.0 |
| HTTP Client | `HttpClient` + `IHttpClientFactory` |
| JSON | `System.Text.Json` |
| Async | Всі методи async з `CancellationToken` |
| Models | C# Records (immutable DTOs) |
| DI | `Microsoft.Extensions.DependencyInjection` |
| Logging | `Microsoft.Extensions.Logging` |
| Testing | xUnit + Moq + FluentAssertions + MockHttp |

## Key Files

- `src/GarminConnect/Api/Endpoints.cs` - всі URL endpoints Garmin API
- `src/GarminConnect/Api/GarminApiClient.cs` - низькорівневий HTTP клієнт
- `src/GarminConnect/Auth/GarminAuthenticator.cs` - OAuth автентифікація
- `src/GarminConnect/Auth/OAuth/FileTokenStore.cs` - збереження токенів
- `src/GarminConnect/Extensions/ServiceCollectionExtensions.cs` - DI extensions
- `src/GarminConnect/GarminClientOptions.cs` - конфігурація клієнта
- `src/GarminConnect/Exceptions/` - ієрархія виключень

## Gotchas

- `CircuitBreaker.SamplingDuration` must be >= 2x `AttemptTimeout.Timeout` (validation error otherwise)
- `IOAuthTokenStore` interface has 4 methods: `LoadAsync`, `SaveAsync`, `ClearAsync`, `ExistsAsync`
- `FileTokenStore` implements `IDisposable` - dispose to release SemaphoreSlim
- `FileTokenStore` accepts optional `ILogger<FileTokenStore>` for error logging
- Resilience options (`MaxRetryAttempts`, `RetryDelay`, `Timeout`) are read from `GarminClientOptions`

## Reference

- Python оригінал: `D:\repositories\PET_PFOJECTS\python-garminconnect`
- План реалізації: `docs/IMPLEMENTATION_PLAN.md`
