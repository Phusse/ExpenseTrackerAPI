using System.Security.Claims;
using ExpenseTracker.Contracts;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Dashboard;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
public class DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger) : ControllerBase
{
    private readonly IDashboardService _dashboardService = dashboardService;
    private readonly ILogger<DashboardController> _logger = logger;

    [HttpGet(ApiRoutes.Dashboard.Get.Summary)]
    public async Task<IActionResult> GetSummary()
    {
        Guid userId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

        if (userId == Guid.Empty)
            return Unauthorized(ApiResponse<object?>.Failure(null, "Invalid user token."));

        try
        {
            DashboardSummaryResponse summary = await _dashboardService.GetDashboardSummaryAsync(userId);

            return Ok(ApiResponse<DashboardSummaryResponse>.Success(summary, "Dashboard summary retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get dashboard summary for {userId}: {message}", [userId, ex.Message]);
            return StatusCode(
                500,
                ApiResponse<object?>.Failure(
                    null,
                    "An error occurred while processing your dashboard summary.",
                    [ex.Message]
                )
            );
        }
    }
}