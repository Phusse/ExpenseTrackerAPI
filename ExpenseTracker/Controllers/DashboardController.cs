using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Dashboards;
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
    /// Gets a summary of the user's financial dashboard for the current month.
    /// </summary>
    /// <remarks>
    /// This endpoint aggregates the user's financial data—such as total expenses, budgets, savings, and goals—into a single monthly view.
    /// Returns a <c>401 response</c> if the user is unauthenticated, and <c>500 response</c> if an internal error occurs during data retrieval.
    /// </remarks>
    /// <returns>
    /// Returns a 200 OK response containing the dashboard summary if successful.
    /// Returns a 401 Unauthorized if the user ID cannot be parsed.
    /// Returns a 500 Internal Server Error if an exception occurs.
    /// </returns>
    /// <response code="200">Dashboard summary retrieved successfully.</response>
    /// <response code="401">Unauthorized access due to invalid or missing token.</response>
    /// <response code="500">Internal server error occurred while processing the request.</response>
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
            var response = ApiResponse<object?>.Fail(null, "An error occured while processing your dashboard summary.", [ex.Message]);

            return StatusCode(500, response);
        }
    }
}
