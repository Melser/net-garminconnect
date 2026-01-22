# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**net-garminconnect** - .NET 8+ бібліотека для взаємодії з Garmin Connect API.

Це C# порт Python бібліотеки [python-garminconnect](https://github.com/cyberjunky/python-garminconnect).

## Project Status

**Поточна фаза:** Фаза 2 - Автентифікація

**Завершено:**
- ✅ Фаза 1: Базова інфраструктура (solution, projects, exceptions, HTTP client, endpoints)

**В процесі:**
- 🔄 Фаза 2: Автентифікація (OAuth flow, Token Store, MFA)

## Project Structure

```
net-garminconnect/
├── GarminConnect.sln
├── src/GarminConnect/
│   ├── Api/
│   │   ├── Endpoints.cs           # URL endpoints для Garmin API
│   │   ├── IGarminApiClient.cs    # Інтерфейс HTTP клієнта
│   │   └── GarminApiClient.cs     # Реалізація HTTP клієнта
│   ├── Auth/                      # (TODO) OAuth, MFA, TokenStore
│   ├── Exceptions/
│   │   ├── GarminConnectException.cs
│   │   ├── GarminConnectAuthenticationException.cs
│   │   ├── GarminConnectConnectionException.cs
│   │   ├── GarminConnectTooManyRequestsException.cs
│   │   └── GarminConnectInvalidFileFormatException.cs
│   ├── Models/                    # (TODO) DTO records
│   └── Services/                  # (TODO) GarminClient partial classes
└── tests/GarminConnect.Tests/
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
- `src/GarminConnect/Exceptions/` - ієрархія виключень

## Reference

- Python оригінал: `D:\repositories\PET_PFOJECTS\python-garminconnect`
- План реалізації: `docs/IMPLEMENTATION_PLAN.md`
