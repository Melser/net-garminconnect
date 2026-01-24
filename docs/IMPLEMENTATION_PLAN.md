# План: Переписування python-garminconnect на C#

## Огляд
Переписати Python бібліотеку `python-garminconnect` на C# як `GarminConnect.NET` - .NET 8+ бібліотеку для взаємодії з Garmin Connect API.

**Рішення:**
- Тільки глобальний сервер (garmin.com, без garmin.cn)
- Базові методи спочатку (~30): Auth + Health + Activities
- FIT протокол включити одразу

---

## Структура проекту

```
net-garminconnect/
├── GarminConnect.sln
├── src/GarminConnect/
│   ├── GarminConnect.csproj
│   ├── IGarminClient.cs              # Головний інтерфейс (partial)
│   ├── GarminClient.cs               # Реалізація (partial)
│   ├── GarminClientOptions.cs
│   ├── Auth/                         # OAuth, MFA, TokenStore
│   ├── Api/                          # HTTP клієнт, Endpoints
│   ├── Models/                       # DTO (records)
│   ├── Services/                     # Partial class implementations
│   ├── Fit/                          # FIT протокол кодування
│   │   ├── FitEncoder.cs
│   │   ├── FitEncoderWeight.cs
│   │   └── FitEncoderBloodPressure.cs
│   ├── Exceptions/                   # Виключення
│   └── Extensions/                   # DI extensions
└── tests/GarminConnect.Tests/
    └── Unit/, Integration/, Fixtures/
```

---

## Фази реалізації

### Фаза 1: Базова інфраструктура ✅ ЗАВЕРШЕНО
- [x] Створити solution та проекти (`dotnet new sln`, `dotnet new classlib`, `dotnet new xunit`)
- [x] Налаштувати .csproj (TargetFramework net8.0, Nullable enable)
- [x] Створити базові класи виключень
- [x] Реалізувати `GarminApiClient` (HttpClient обгортка)
- [x] Додати `Endpoints.cs` з URL константами

### Фаза 2: Автентифікація ✅ ЗАВЕРШЕНО
- [x] Дослідити OAuth flow (аналіз Garth Python бібліотеки)
- [x] Реалізувати `IGarminAuthenticator` / `GarminAuthenticator`
- [x] Додати `IOAuthTokenStore` / `FileTokenStore`
- [x] Реалізувати MFA handling (`IMfaHandler`)
- [x] Тестування логіну та збереження токенів
- [x] Unit tests (64 тести)

### Фаза 3: Основні API (Health & Activities) ✅ ЗАВЕРШЕНО
- [x] Моделі: `DailySummary`, `HeartRateData`, `SleepData`, `Activity`, `ActivityDetails`, `BodyBattery`, `Stress`, `UserProfile`
- [x] `GarminClient.Health.cs`: GetDailySummary, GetHeartRates, GetSleep, GetStress, GetBodyBattery
- [x] `GarminClient.Activity.cs`: GetActivities, GetActivityDetails, Download/Upload, Delete
- [x] `GarminClient.User.cs`: GetDisplayName, GetFullName, GetUnitSystem, GetUserProfile, GetUserSettings
- [x] Виправлено: кешування користувача (очищення при logout)
- [x] Виправлено: захист від нескінченного циклу в пагінації
- [x] Unit tests (44 нових тести, 108 всього)

### Фаза 4: FIT протокол ✅ ЗАВЕРШЕНО
- [x] `FitEncoder.cs` - базове кодування FIT (CRC, header, records)
- [x] `FitEncoderWeight.cs` - кодування ваги та body composition
- [x] `FitEncoderBloodPressure.cs` - кодування артеріального тиску
- [x] Інтеграція з GarminClient: `AddBodyCompositionAsync`, `AddBloodPressureAsync`
- [x] Unit tests (21 новий тест, 129 всього)

### Фаза 5: Розширені API ✅ ЗАВЕРШЕНО
- [x] Body Composition & Weight - GetBodyComposition, GetDailyWeighIns, DeleteWeighIn
- [x] Devices (6 методів) - GetDevices, GetDeviceSettings, GetPrimaryTrainingDevice, GetDeviceSolarData, GetDeviceLastUsed, GetDeviceAlarms
- [x] Gear (7 методів) - GetGear, GetGearStats, GetGearDefaults, SetGearDefault, GetGearActivities, AddGearToActivity
- [x] Workouts (6 методів) - GetWorkouts, GetWorkoutById, DownloadWorkout, UploadWorkout, GetScheduledWorkoutById
- [x] Badges (6 методів) - GetEarnedBadges, GetAvailableBadges, GetInProgressBadges, GetBadgeChallenges, GetAvailable/NonCompletedBadgeChallenges
- [x] Goals (1 метод) - GetGoals
- [x] Training Plans (3 методи) - GetTrainingPlans, GetTrainingPlanById, GetAdaptiveTrainingPlanById

### Фаза 6: DI та Resilience ✅ ЗАВЕРШЕНО
- [x] `GarminClientOptions.cs` - конфігурація клієнта
- [x] `ServiceCollectionExtensions` для DI з методами:
  - `AddGarminConnect()` - базова реєстрація
  - `AddGarminConnect<TMfaHandler>()` - з кастомним MFA handler
  - `AddGarminConnect(Func<...> mfaCallback)` - з callback-based MFA
- [x] Retry policies (Microsoft.Extensions.Http.Resilience):
  - Exponential backoff з jitter
  - Retry на 429 та 5xx помилки
- [x] Circuit Breaker policy
- [x] Timeout policies (attempt + total)
- [x] Logging integration
- [x] Unit tests (15 нових тестів, 144 всього)

### Фаза 7: Тестування та документація
- [ ] Unit tests (>80% coverage)
- [ ] Integration tests з MockHttp
- [ ] XML документація
- [ ] README з прикладами

---

## Ключові архітектурні рішення

| Аспект | Рішення |
|--------|---------|
| Target Framework | .NET 8.0+ |
| HTTP Client | `HttpClient` + `IHttpClientFactory` |
| JSON | `System.Text.Json` з source generators |
| Async | Всі методи async з `CancellationToken` |
| Models | C# Records (immutable DTOs) |
| DI | `Microsoft.Extensions.DependencyInjection` |
| Logging | `Microsoft.Extensions.Logging` |
| Testing | xUnit + Moq + FluentAssertions + MockHttp |

---

## Критичні файли для модифікації/створення

1. `src/GarminConnect/IGarminClient.cs` - головний інтерфейс
2. `src/GarminConnect/GarminClient.cs` - реалізація
3. `src/GarminConnect/Auth/GarminAuthenticator.cs` - OAuth flow
4. `src/GarminConnect/Api/GarminApiClient.cs` - HTTP клієнт
5. `src/GarminConnect/Api/Endpoints.cs` - URL endpoints

---

## NuGet залежності

```xml
<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.*" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.*" />
<PackageReference Include="Microsoft.Extensions.Http.Resilience" Version="8.0.*" />
```

---

## Верифікація

1. **Build:** `dotnet build` - має компілюватися без помилок
2. **Tests:** `dotnet test` - всі тести мають проходити
3. **Manual test (direct):**
   ```csharp
   var client = new GarminClient();
   await client.LoginAsync("email", "password");
   var summary = await client.GetDailySummaryAsync(DateOnly.FromDateTime(DateTime.Today));
   ```
4. **Manual test (DI):**
   ```csharp
   services.AddGarminConnect(options =>
   {
       options.TokenStorePath = "garmin_tokens.json";
       options.MaxRetryAttempts = 5;
   });

   // In your service:
   public class MyService(IGarminClient garminClient)
   {
       public async Task DoWork()
       {
           var summary = await garminClient.GetDailySummaryAsync(DateOnly.FromDateTime(DateTime.Today));
       }
   }
   ```
5. **Pack:** `dotnet pack -c Release` - має створювати NuGet пакет

---

## Оцінка обсягу

- **~40 моделей** (records для API responses)
- **~127 методів API** (partial classes)
- **~15 файлів інфраструктури** (Auth, API, Exceptions, Extensions)
- **144 unit tests** (всі проходять)

---

## Поточний стан

| Метрика | Значення |
|---------|----------|
| Фази завершено | 6 з 7 |
| Тестів | 144 |
| Методів API | ~62 реалізовано |

**Наступний крок:** Фаза 7 - Тестування та документація
