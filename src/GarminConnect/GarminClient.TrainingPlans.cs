using System.Text.Json;
using GarminConnect.Api;

namespace GarminConnect;

public sealed partial class GarminClient
{
    #region Training Plans

    /// <summary>
    /// Gets all training plans.
    /// </summary>
    public async Task<JsonElement> GetTrainingPlansAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        return await _apiClient.GetAsync<JsonElement>(Endpoints.TrainingPlans, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a training plan by ID.
    /// </summary>
    /// <param name="planId">Training plan ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetTrainingPlanByIdAsync(long planId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(planId);

        var url = string.Format(Endpoints.TrainingPlanById, planId);
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets an adaptive training plan by ID.
    /// </summary>
    /// <param name="planId">Training plan ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetAdaptiveTrainingPlanByIdAsync(long planId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(planId);

        var url = string.Format(Endpoints.AdaptiveTrainingPlanById, planId);
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    #endregion
}
