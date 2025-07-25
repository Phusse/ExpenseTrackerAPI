using ExpenseTracker.Models.DTOs.Dashboards;

namespace ExpenseTracker.Services;

/// <summary>
/// Defines methods for retrieving dashboard-related data for users.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Retrieves a summary of the dashboard for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a
    /// <see cref="DashboardSummaryResponse"/> object with the user's total expenses, savings, and budget status.
    /// </returns>
    Task<DashboardSummaryResponse> GetDashboardSummaryAsync(Guid userId);
}
