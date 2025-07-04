using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Enums;
using ExpenseTracker.Contracts;
using System.Security.Claims;

[ApiController]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpPost]
    [Route(ExpenseRoutes.BudgetPostUrl.Create)]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Invalid user token." });

        var result = await _budgetService.CreateBudgetAsync(request, userId);

        if (!result.IsSuccess)
            return Conflict(new { message = result.ErrorMessage });

        return Created($"{ExpenseRoutes.BudgetBase}?category={request.Category}&month={request.Month}&year={request.Year}", result.Data);
    }

    [HttpGet]
    [Route(ExpenseRoutes.BudgetGetUrl.Summary)]
    public async Task<IActionResult> GetBudgetSummary(
        [FromQuery] ExpenseCategory category,
        [FromQuery] int month,
        [FromQuery] int year)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Invalid user token." });

        var summary = await _budgetService.GetBudgetSummaryAsync(userId, category, month, year);

        if (summary.BudgetedAmount == 0)
            return NotFound(new { message = "No budget available for this category and month." });

        return Ok(summary);
    }

    [HttpGet]
    [Route(ExpenseRoutes.BudgetGetUrl.Status)]
    public async Task<IActionResult> GetBudgetStatus(
        [FromQuery] ExpenseCategory category,
        [FromQuery] int month,
        [FromQuery] int year)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Invalid user token." });

        var (budgeted, spent) = await _budgetService.GetBudgetStatusAsync(userId, category, month, year);

        if (budgeted == 0)
            return NotFound(new { message = "No budget set for the selected category and time range." });

        return Ok(new
        {
            Budgeted = budgeted,
            Spent = spent,
            Remaining = budgeted - spent,
            PercentageUsed = Math.Round((spent / budgeted) * 100, 2)
        });
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim?.Value, out var userId) ? userId : Guid.Empty;
    }
}
