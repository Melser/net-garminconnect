# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**net-garminconnect** - .NET бібліотека для взаємодії з Garmin Connect API.

## Project Status

Проект знаходиться на початковій стадії розробки. Структура та код ще не створені.

## Recommended Project Setup

При створенні проекту рекомендується:

```bash
# Створення solution та проектів
dotnet new sln -n GarminConnect
dotnet new classlib -n GarminConnect -o src/GarminConnect
dotnet new xunit -n GarminConnect.Tests -o tests/GarminConnect.Tests

# Додавання проектів до solution
dotnet sln add src/GarminConnect/GarminConnect.csproj
dotnet sln add tests/GarminConnect.Tests/GarminConnect.Tests.csproj

# Додавання референсу на основний проект в тестовому
dotnet add tests/GarminConnect.Tests/GarminConnect.Tests.csproj reference src/GarminConnect/GarminConnect.csproj
```

## Common Commands (after setup)

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
