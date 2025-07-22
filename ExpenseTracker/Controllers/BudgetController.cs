using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Enums;
using ExpenseTracker.Services;
using ExpenseTracker.Models.DTOs.Budget;
using ExpenseTracker.Utilities.Routing;
using ExpenseTracker.Utilities.Extension;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers;

[Authorize]
[ApiController]
public class BudgetController(IBudgetService budgetService) : ControllerBase
{
    private readonly IBudgetService _budgetService = budgetService;

    [HttpPost(ApiRoutes.Budget.Post.Create)]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetRequest request)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing token."));
        }

        ServiceResult<CreateBudgetResponse?> result = await _budgetService.CreateBudgetAsync(request, userId);

        if (result.Success)
        {
            return Created(
                $"{ApiRoutes.Budget.Post.Create}?category={request.Category}&period={request.Period}",
                ApiResponse<CreateBudgetResponse>.Ok(result.Data, "Budget created successfully.")
            );
        }

        return Conflict(ApiResponse<object?>.Fail(null, result.Message, result.Errors));
    }

    [HttpGet(ApiRoutes.Budget.Get.Summary)]
    public async Task<IActionResult> GetBudgetSummary([FromQuery] ExpenseCategory category, [FromQuery] DateOnly period)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing token."));
        }

        BudgetSummaryResponse summary = await _budgetService.GetBudgetSummaryAsync(userId, category, period);

        if (summary.BudgetedAmount == 0)
        {
            return NotFound(ApiResponse<object?>.Fail(null, "No budget available for this category and month."));
        }

        return Ok(ApiResponse<BudgetSummaryResponse>.Ok(summary, "Budget summary retrieved successfully."));
    }

    [HttpGet(ApiRoutes.Budget.Get.Status)]
    public async Task<IActionResult> GetBudgetStatus([FromQuery] ExpenseCategory category, [FromQuery] DateOnly period)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing token."));
        }

        BudgetStatusResponse result = await _budgetService.GetBudgetStatusAsync(userId, category, period);

        if (result.BudgetedAmount == 0)
        {
            return NotFound(ApiResponse<object?>.Fail(null, "No budget set for the selected category and time range."));
        }

        BudgetSummaryResponse summary = new()
		{
            Category = category,
            Period = period,
            BudgetedAmount = result.BudgetedAmount,
            SpentAmount = result.SpentAmount,
        };

        return Ok(ApiResponse<BudgetSummaryResponse>.Ok(summary, "Budget summary retrieved."));
    }
}
