using System.Text.Json;
using GarminConnect.Api;

namespace GarminConnect;

public sealed partial class GarminClient
{
    #region Goals

    /// <summary>
    /// Gets user goals.
    /// </summary>
    /// <param name="goalType">Goal type filter (optional). Common values: "step", "distance", "calorie".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetGoalsAsync(string? goalType = null, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = Endpoints.Goals;
        if (!string.IsNullOrEmpty(goalType))
        {
            url += $"?goalType={Uri.EscapeDataString(goalType)}";
        }

        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    #endregion
}
