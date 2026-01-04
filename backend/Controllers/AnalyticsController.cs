using ExpenseTracker.Models.DTOs.Analytics;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController(AnalyticsService analyticsService) : ControllerBase
{
    private readonly AnalyticsService _analyticsService = analyticsService;

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Get financial health score (0-100)
    /// </summary>
    [HttpGet("health-score")]
    public async Task<ActionResult<FinancialHealthScoreDto>> GetHealthScore()
    {
        try
        {
            var userId = GetUserId();
            var score = await _analyticsService.GetFinancialHealthScoreAsync(userId);
            return Ok(score);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to calculate health score", error = ex.Message });
        }
    }

    /// <summary>
    /// Get spending patterns analysis
    /// </summary>
    [HttpGet("spending-patterns")]
    public async Task<ActionResult<SpendingPatternsDto>> GetSpendingPatterns()
    {
        try
        {
            var userId = GetUserId();
            var patterns = await _analyticsService.GetSpendingPatternsAsync(userId);
            return Ok(patterns);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to analyze spending patterns", error = ex.Message });
        }
    }

    /// <summary>
    /// Get category trends
    /// </summary>
    [HttpGet("category-trends")]
    public async Task<ActionResult<List<CategoryTrendDto>>> GetCategoryTrends()
    {
        try
        {
            var userId = GetUserId();
            var trends = await _analyticsService.GetCategoryTrendsAsync(userId);
            return Ok(trends);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to get category trends", error = ex.Message });
        }
    }

    /// <summary>
    /// Get spending forecast
    /// </summary>
    [HttpGet("forecast")]
    public async Task<ActionResult<SpendingForecastDto>> GetForecast()
    {
        try
        {
            var userId = GetUserId();
            var forecast = await _analyticsService.GetSpendingForecastAsync(userId);
            return Ok(forecast);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to generate forecast", error = ex.Message });
        }
    }

    /// <summary>
    /// Get predictive insights and recommendations
    /// </summary>
    [HttpGet("predictions")]
    public async Task<ActionResult<PredictiveInsightsDto>> GetPredictions()
    {
        try
        {
            var userId = GetUserId();
            var insights = await _analyticsService.GetPredictiveInsightsAsync(userId);
            return Ok(insights);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to generate predictions", error = ex.Message });
        }
    }
}
