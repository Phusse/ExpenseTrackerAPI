using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Services;
using ExpenseTracker.Models.DTOs.Budget;
using ExpenseTracker.Utilities.Routing;
using ExpenseTracker.Utilities.Extension;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Budgets;

namespace ExpenseTracker.Controllers;

/// <summary>
/// Handles creation, updates, deletion, and retrieval of user budget data.
/// </summary>
[Authorize]
[ApiController]
public class BudgetController(IBudgetService budgetService) : ControllerBase
{
    private readonly IBudgetService _budgetService = budgetService;

    /// <summary>
    /// Adds a new budget for a specific category and period.
    /// </summary>
    /// <remarks>
    /// Creates a budget entry tied to a user's account using the provided category and time period.
    /// If a budget already exists for the same category-period pair, the request will be rejected to avoid duplication.
    /// </remarks>
    /// <param name="request">The data required to create the budget.</param>
    /// <returns>A <see cref="CreateBudgetResponse"/> with the created budget details.</returns>
    /// <response code="201">Budget created successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="409">A budget for the same category and period already exists.</response>
    [HttpPost(ApiRoutes.Budget.Post.Create)]
    [ProducesResponseType(typeof(ApiResponse<CreateBudgetResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetRequest request)
    {
        if (!User.TryGetUserId(out Guid userId))
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing token."));

        var result = await _budgetService.CreateBudgetAsync(request, userId);

        if (result.Success)
        {
            return Created(
                $"{ApiRoutes.Budget.Post.Create}?category={request.Category}&period={request.Period}",
                ApiResponse<CreateBudgetResponse>.Ok(result.Data, "Budget created successfully.")
            );
        }

        return Conflict(ApiResponse<object?>.Fail(null, result.Message, result.Errors));
    }

    /// <summary>
    /// Returns usage summary for a specific budget.
    /// </summary>
    /// <remarks>
    /// Retrieves the budgeted amount and spending progress for a given category and month.
    /// If no budget exists for the input, a <c>404 response</c> is returned to indicate absence of data.
    /// </remarks>
    /// <param name="request">The budget category and period to evaluate.</param>
    /// <returns>A <see cref="BudgetSummaryResponse"/> containing usage details.</returns>
    /// <response code="200">Budget summary retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">No budget found for the given category and period.</response>
    [HttpGet(ApiRoutes.Budget.Get.Status)]
    [ProducesResponseType(typeof(ApiResponse<BudgetSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBudgetStatus([FromQuery] BudgetStatusRequest request)
    {
        if (!User.TryGetUserId(out Guid userId))
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing token."));

        BudgetStatusResponse result = await _budgetService.GetBudgetStatusAsync(userId, request);

        if (result.BudgetedAmount == 0)
        {
            return NotFound(ApiResponse<object?>.Fail(null, "No budget set for the selected category and time range."));
        }

        BudgetSummaryResponse summary = new()
        {
            Id = result.Id,
            Category = request.Category!.Value,
            Period = new DateOnly(request.Period!.Value.Year, request.Period.Value.Month, 1),
            BudgetedAmount = result.BudgetedAmount,
            SpentAmount = result.SpentAmount
        };

        return Ok(ApiResponse<BudgetSummaryResponse>.Ok(summary, "Budget summary retrieved."));
    }

    /// <summary>
    /// Returns an overview of all budget categories for a given month.
    /// </summary>
    /// <remarks>
    /// Provides a top-level summary of budget performance across all user-defined categories for the specified period.
    /// This helps users identify areas of overspending or underutilization.
    /// </remarks>
    /// <param name="period">The month and year to retrieve the overview for.</param>
    /// <returns>A <see cref="BudgetOverviewSummaryResponse"/> containing performance insights.</returns>
    /// <response code="200">Overview retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet(ApiRoutes.Budget.Get.Overview)]
    [ProducesResponseType(typeof(ApiResponse<BudgetOverviewSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetBudgetOverview([FromQuery] DateOnly period)
    {
        if (!User.TryGetUserId(out Guid userId))
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing token."));

        BudgetOverviewSummaryResponse result = await _budgetService.GetBudgetOverviewAsync(userId, period);
        return Ok(ApiResponse<BudgetOverviewSummaryResponse>.Ok(result, "Budget overview retrieved successfully."));
    }

    /// <summary>
    /// Modifies an existing budget entry by ID.
    /// </summary>
    /// <remarks>
    /// Allows users to adjust budget values (amount, category, or period) using the budget's unique identifier.
    /// If the budget does not exist, a <c>404 response</c> is returned. If the update violates any business rule or causes a conflict (e.g., duplicate budget), a <c>409 response</c> is issued.
    /// </remarks>
    /// <param name="budgetId">The unique ID of the budget entry.</param>
    /// <param name="request">The data to update (limit, category, or period).</param>
    /// <returns>An operation result indicating success or failure.</returns>
    /// <response code="200">Budget updated successfully.</response>
    /// <response code="400">No update values provided in the request body.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">No budget found with the provided ID.</response>
    /// <response code="409">Update failed due to data conflict or rule violation.</response>
    [HttpPut(ApiRoutes.Budget.Put.Update)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBudget([FromRoute(Name = "id")] Guid budgetId, [FromBody] UpdateBudgetRequest request)
    {
        if (!User.TryGetUserId(out Guid userId))
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing token."));

        ServiceResult<object?> result = await _budgetService.UpdateBudgetAsync(userId, budgetId, request);

        if (result.Success)
            return Ok(ApiResponse<object?>.Ok(null, result.Message ?? "Budget updated successfully."));

        if (result.Message == "Budget not found.")
            return NotFound(ApiResponse<object?>.Fail(null, result.Message, result.Errors));

        return Conflict(ApiResponse<object?>.Fail(null, result.Message, result.Errors));
    }

    /// <summary>
    /// Removes a budget by its unique ID.
    /// </summary>
    /// <remarks>
    /// Deletes a user's budget entry from the system. If no budget matches the given ID, a <c>404 response</c> is returned.
    /// This operation is irreversible and should be used cautiously.
    /// </remarks>
    /// <param name="budgetId">The unique ID of the budget entry to delete.</param>
    /// <returns>An operation result indicating success or failure.</returns>
    /// <response code="200">Budget deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">No budget entry found with the provided ID.</response>
    [HttpDelete(ApiRoutes.Budget.Delete.Remove)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBudget([FromRoute(Name = "id")] Guid budgetId)
    {
        if (!User.TryGetUserId(out Guid userId))
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing token."));

        var result = await _budgetService.DeleteBudgetAsync(userId, budgetId);

        if (result.Success)
            return Ok(ApiResponse<object?>.Ok(null, result.Message ?? "Budget deleted successfully."));

        return NotFound(ApiResponse<object?>.Fail(null, result.Message, result.Errors));
    }
}
