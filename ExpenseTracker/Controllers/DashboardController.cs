using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Dashboard;
using ExpenseTracker.Services;
using ExpenseTracker.Utilities.Extension;
using ExpenseTracker.Utilities.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

/// <summary>
/// Provides endpoints for accessing user dashboard data such as expenses, savings, budgets, and goals.
/// </summary>
[Authorize]
[ApiController]
public class DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger) : ControllerBase
{
    private readonly IDashboardService _dashboardService = dashboardService;
    private readonly ILogger<DashboardController> _logger = logger;

    /// <summary>
    /// Retrieves a summary of the user's dashboard data for the current month.
    /// </summary>
    /// <returns>
    /// Returns a 200 OK response containing the dashboard summary if successful.
    /// Returns a 401 Unauthorized if the user ID cannot be parsed.
    /// Returns a 500 Internal Server Error if an exception occurs.
    /// </returns>
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status500InternalServerError)]
    [HttpGet(ApiRoutes.Dashboard.Get.Summary)]
    public async Task<IActionResult> GetSummary()
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid user token."));
        }

        try
        {
            DashboardSummaryResponse summary = await _dashboardService.GetDashboardSummaryAsync(userId);

            return Ok(ApiResponse<DashboardSummaryResponse>.Ok(summary, "Dashboard summary retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get dashboard summary for {userId}: {message}", [userId, ex.Message]);

            return StatusCode(
                500,
                ApiResponse<object?>.Fail(
                    null,
                    "An error occurred while processing your dashboard summary.",
                    [ex.Message]
                )
            );
        }
    }
}
