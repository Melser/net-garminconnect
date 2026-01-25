# GarminConnect.NET

A .NET 8+ library for interacting with the Garmin Connect API.

This is a C# port of the Python library [python-garminconnect](https://github.com/cyberjunky/python-garminconnect).

## Features

- OAuth2 authentication with token persistence
- MFA (Multi-Factor Authentication) support
- Health data: daily summary, heart rate, sleep, stress, body battery
- Activities: list, details, download (FIT/TCX/GPX), upload
- Body composition & weight management
- Devices, gear, workouts, badges, goals, training plans
- Dependency injection support with `IServiceCollection`
- Resilience: retry policies, circuit breaker, timeout handling

## Installation

```bash
dotnet add package GarminConnect
```

## Quick Start

### Basic Usage

```csharp
using GarminConnect;

// Create client
using var client = new GarminClient();

// Login
var result = await client.LoginAsync("your@email.com", "your-password");

if (result.RequiresMfa)
{
    Console.Write("Enter MFA code: ");
    var mfaCode = Console.ReadLine();
    await client.CompleteMfaAsync(mfaCode!);
}

// Get today's summary
var today = DateOnly.FromDateTime(DateTime.Today);
var summary = await client.GetDailySummaryAsync(today);
Console.WriteLine($"Steps: {summary?.TotalSteps}");

// Get activities
var activities = await client.GetActivitiesAsync(0, 10);
foreach (var activity in activities)
{
    Console.WriteLine($"{activity.ActivityName}: {activity.Distance}m");
}
```

### With Token Persistence

```csharp
using GarminConnect;
using GarminConnect.Auth.OAuth;

// Use FileTokenStore to persist tokens between sessions
var tokenStore = new FileTokenStore("garmin_tokens.json");

using var client = new GarminClient(tokenStore);

// Try to resume session from stored tokens
if (await client.ResumeSessionAsync())
{
    Console.WriteLine("Session resumed!");
}
else
{
    // Need to login
    await client.LoginAsync("your@email.com", "your-password");
}
```

### With Dependency Injection

```csharp
using GarminConnect.Extensions;

// In your Startup.cs or Program.cs
services.AddGarminConnect(options =>
{
    options.TokenStorePath = "garmin_tokens.json";
    options.Timeout = TimeSpan.FromSeconds(60);
    options.MaxRetryAttempts = 5;
});

// Or with custom MFA handler
services.AddGarminConnect<MyMfaHandler>(options =>
{
    options.TokenStorePath = "garmin_tokens.json";
});

// Or with callback-based MFA
services.AddGarminConnect(
    async ct =>
    {
        Console.Write("Enter MFA code: ");
        return Console.ReadLine();
    },
    options => options.TokenStorePath = "tokens.json"
);
```

Then inject `IGarminClient` in your services:

```csharp
public class FitnessService(IGarminClient garminClient)
{
    public async Task<int> GetTodayStepsAsync()
    {
        var summary = await garminClient.GetDailySummaryAsync(DateOnly.FromDateTime(DateTime.Today));
        return summary?.TotalSteps ?? 0;
    }
}
```

## API Examples

### Health Data

```csharp
var today = DateOnly.FromDateTime(DateTime.Today);

// Daily summary (steps, calories, distance, etc.)
var summary = await client.GetDailySummaryAsync(today);

// Heart rate data
var heartRate = await client.GetHeartRatesAsync(today);
var resting = await client.GetRestingHeartRateAsync(today);

// Sleep data
var sleep = await client.GetSleepDataAsync(today);

// Stress levels
var stress = await client.GetStressDataAsync(today);

// Body battery
var battery = await client.GetBodyBatteryAsync(today);
```

### Activities

```csharp
// List activities with pagination
var activities = await client.GetActivitiesAsync(start: 0, limit: 20);

// Get activities by date range
var recentActivities = await client.GetActivitiesByDateAsync(
    new DateOnly(2024, 1, 1),
    new DateOnly(2024, 1, 31)
);

// Get activity details
var details = await client.GetActivityDetailsAsync(activityId);

// Download activity file
byte[] fitFile = await client.DownloadActivityAsync(activityId, ActivityFileFormat.Fit);
File.WriteAllBytes("activity.fit", fitFile);

// Upload activity
byte[] newActivity = File.ReadAllBytes("my_activity.fit");
long uploadedId = await client.UploadActivityAsync(newActivity, "my_activity.fit");

// Delete activity
await client.DeleteActivityAsync(activityId);
```

### Body Composition

```csharp
// Add weight measurement
await client.AddBodyCompositionAsync(
    weight: 75.5,
    timestamp: DateTime.Now,
    percentFat: 18.5,
    muscleMass: 35.0
);

// Get weight history
var weightData = await client.GetBodyCompositionAsync(
    new DateOnly(2024, 1, 1),
    new DateOnly(2024, 1, 31)
);

// Add blood pressure
await client.AddBloodPressureAsync(
    systolicPressure: 120,
    diastolicPressure: 80,
    heartRate: 65
);
```

### Devices & Gear

```csharp
// Get devices
var devices = await client.GetDevicesAsync();

// Get device settings
var settings = await client.GetDeviceSettingsAsync("deviceId");

// Get gear
var gear = await client.GetGearAsync("userProfileNumber");

// Add gear to activity
await client.AddGearToActivityAsync("gearUuid", activityId);
```

### Workouts

```csharp
// Get workouts
var workouts = await client.GetWorkoutsAsync(0, 100);

// Get specific workout
var workout = await client.GetWorkoutByIdAsync(workoutId);

// Download workout as FIT
byte[] workoutFit = await client.DownloadWorkoutAsync(workoutId);
```

### Badges & Goals

```csharp
// Get badges
var earnedBadges = await client.GetEarnedBadgesAsync();
var availableBadges = await client.GetAvailableBadgesAsync();
var inProgressBadges = await client.GetInProgressBadgesAsync();

// Get goals
var goals = await client.GetGoalsAsync();
var stepGoals = await client.GetGoalsAsync("steps");
```

## Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `BaseUrl` | `https://connect.garmin.com` | Garmin Connect API base URL |
| `TokenStorePath` | `null` | Path to store authentication tokens |
| `Timeout` | `30s` | HTTP request timeout |
| `MaxRetryAttempts` | `3` | Max retry attempts for transient failures |
| `RetryDelay` | `1s` | Base delay between retries (exponential backoff) |
| `AutoRefreshToken` | `true` | Automatically refresh expired tokens |
| `UserAgent` | `GCM-iOS-5.7.2.1` | User-Agent header for requests |

## Resilience

The library includes built-in resilience using `Microsoft.Extensions.Http.Resilience`:

- **Retry Policy**: Automatic retry with exponential backoff for transient failures (429, 5xx errors)
- **Circuit Breaker**: Prevents cascading failures when the API is down
- **Timeout**: Configurable timeouts per request and total operation

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

MIT License - see [LICENSE](LICENSE) for details.

## Acknowledgments

- [python-garminconnect](https://github.com/cyberjunky/python-garminconnect) - Original Python implementation
- [Garth](https://github.com/matin/garth) - Python library for Garmin SSO authentication
