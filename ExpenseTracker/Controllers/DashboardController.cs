using System.Security.Claims;
using ExpenseTracker.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    [Route(ApiRoutes.Dashboard.Get.Summary)]
    public async Task<IActionResult> GetSummary()
    {
        var userId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Invalid user token." });

        try
        {
            var summary = await _dashboardService.GetDashboardSummaryAsync(userId);

            if (summary == null)
                return NotFound(new { message = "Unable to retrieve dashboard summary." });

            return Ok(new
            {
                message = "Dashboard summary retrieved successfully.",
                data = summary
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to get dashboard summary for {userId}: {ex.Message}");
            return StatusCode(500, new
            {
                message = "An error occurred while processing your dashboard summary.",
                error = ex.Message
            });
        }
    }
}