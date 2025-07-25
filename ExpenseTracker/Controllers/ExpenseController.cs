using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using ExpenseTracker.Utilities.Routing;
using ExpenseTracker.Services;
using ExpenseTracker.Models.DTOs.Expenses;
using ExpenseTracker.Utilities.Extension;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers;

/// <summary>
/// Provides endpoints for managing user expenses, including creation, retrieval, filtering, updating, and deletion.
/// </summary>
[Authorize]
[ApiController]
public class ExpenseController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _expenseService = expenseService;

    /// <summary>
    /// Retrieves the ID of the currently authenticated user.
    /// </summary>
    /// <returns>The GUID representing the current user's ID.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user ID cannot be resolved from the token.</exception>
    private Guid GetCurrentUserId()
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }

        return userId;
    }

    /// <summary>
    /// Creates a new expense for the authenticated user.
    /// </summary>
    /// <param name="request">The expense details.</param>
    /// <returns>Returns the created expense.</returns>
    /// <response code="201">Expense created successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">Internal server error.</response>
    [ProducesResponseType(typeof(ApiResponse<CreateExpenseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status500InternalServerError)]
    [HttpPost(ApiRoutes.Expense.Post.Create)]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        try
        {
            Guid userId = GetCurrentUserId();
            CreateExpenseResponse result = await _expenseService.CreateExpenseAsync(userId, request);

            return CreatedAtAction(nameof(CreateExpense), ApiResponse<CreateExpenseResponse>.Ok(result, "Expense created successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, null, [ex.Message]));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object?>.Fail(null, null, [ex.Message]));
        }
    }

    /// <summary>
    /// Retrieves a single expense by its ID.
    /// </summary>
    /// <param name="id">The expense ID.</param>
    /// <returns>Returns the requested expense if found.</returns>
    /// <response code="200">Expense found.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">Expense not found.</response>
    [ProducesResponseType(typeof(ApiResponse<CreateExpenseResponse?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [HttpGet(ApiRoutes.Expense.Get.ById)]
    public async Task<IActionResult> GetExpenseByIdAsync(Guid id)
    {
        try
        {
            Guid userId = GetCurrentUserId();
            CreateExpenseResponse? result = await _expenseService.GetExpenseByIdAsync(id, userId);

            if (result is null)
            {
                return NotFound(ApiResponse<object?>.Fail(null, "Expense not found."));
            }

            return Ok(ApiResponse<CreateExpenseResponse?>.Ok(result, "Expense retrieved successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, null, [ex.Message]));
        }
    }

    /// <summary>
    /// Retrieves all expenses associated with the authenticated user.
    /// </summary>
    /// <returns>A list of all user expenses.</returns>
    /// <response code="200">Expenses retrieved successfully.</response>
    /// <response code="404">No expenses found.</response>
    /// <response code="401">Unauthorized access.</response>
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CreateExpenseResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet(ApiRoutes.Expense.Get.All)]
    public async Task<IActionResult> GetAllExpensesAsync()
    {
        try
        {
            Guid userId = GetCurrentUserId();
            IEnumerable<CreateExpenseResponse> expenses = await _expenseService.GetAllExpensesAsync(userId);

            if (!expenses.Any())
            {
                return NotFound(ApiResponse<object?>.Ok(null, "No expenses found."));
            }

            return Ok(ApiResponse<IEnumerable<CreateExpenseResponse>>.Ok(expenses, "Expenses retrieved successfully."));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    /// <summary>
    /// Retrieves expenses filtered by the provided criteria (date range, category, etc.).
    /// </summary>
    /// <param name="request">The filtering options.</param>
    /// <returns>Filtered list of user expenses.</returns>
    /// <response code="200">Filtered expenses retrieved successfully.</response>
    /// <response code="404">No expenses found.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">Internal server error.</response>
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CreateExpenseResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status500InternalServerError)]
    [HttpGet(ApiRoutes.Expense.Get.Filter)]
    public async Task<IActionResult> GetFilteredExpenses([FromQuery] FilteredExpenseRequest request)
    {
        try
        {
            Guid userId = GetCurrentUserId();
            IEnumerable<CreateExpenseResponse> expenses = await _expenseService.GetFilteredExpensesAsync(userId, request);

            if (!expenses.Any())
            {
                return NotFound(ApiResponse<object?>.Ok(null, "No expenses found."));
            }

            return Ok(ApiResponse<IEnumerable<CreateExpenseResponse>>.Ok(expenses, "Filtered expenses retrieved successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, null, [ex.Message]));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object?>.Fail(null, "An error occurred while retriving expenses.", [ex.Message]));
        }
    }

    /// <summary>
    /// Calculates the total expenses for a given date range or period.
    /// </summary>
    /// <param name="request">The filter request with optional start date, end date, or specific period.</param>
    /// <returns>The total expense within the specified range.</returns>
    /// <response code="200">Total calculated successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">Internal server error.</response>
    [ProducesResponseType(typeof(ApiResponse<double>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status500InternalServerError)]
    [HttpGet(ApiRoutes.Expense.Get.Total)]
    public async Task<IActionResult> GetTotalExpense([FromQuery] TotalExpenseRequest request)
    {
        try
        {
            Guid userId = GetCurrentUserId();
            double total = await _expenseService.GetTotalExpenseAsync(userId, request);

            string message;

            if (request.Period.HasValue)
            {
                DateOnly period = request.Period.Value;
                message = $"Total expense for {period.ToString("MMMM yyyy", CultureInfo.CurrentCulture)} is: {total:C}";
            }
            else if (request.StartDate.HasValue || request.EndDate.HasValue)
            {
                string from = request.StartDate?.ToString("yyyy-MM-dd") ?? "beginning";
                string to = request.EndDate?.ToString("yyyy-MM-dd") ?? "now";
                message = $"Total expense from {from} to {to} is: {total:C}";
            }
            else
            {
                message = $"Total expense is: {total:C}";
            }

            return Ok(ApiResponse<double>.Ok(total, message));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Unauthorized access."));
        }
        catch (Exception ex)
        {
            var errorResponse = ApiResponse<object?>.Fail(
                null,
                "An error occurred while calculating total expense.",
                [ex.Message]
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Updates an existing expense.
    /// </summary>
    /// <param name="id">The ID of the expense to update.</param>
    /// <param name="request">The updated expense details.</param>
    /// <returns>Confirmation of update or error response.</returns>
    /// <response code="200">Expense updated successfully.</response>
    /// <response code="400">Invalid data or ID mismatch.</response>
    /// <response code="404">Expense not found.</response>
    /// <response code="401">Unauthorized access.</response>
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [HttpPut(ApiRoutes.Expense.Put.Update)]
    public async Task<IActionResult> UpdateExpenseAsync(Guid id, [FromBody] UpdateExpenseRequest request)
    {
        try
        {
            Guid userId = GetCurrentUserId();

            if (request is null || id != request.Id)
            {
                return BadRequest(ApiResponse<object?>.Fail(null, "Invalid data or ID mismatch."));
            }

            bool result = await _expenseService.UpdateExpenseAsync(userId, request);

            if (!result)
            {
                return NotFound(ApiResponse<object?>.Fail(null, $"Expense with ID {id} not found"));
            }

            return Ok(ApiResponse<object?>.Ok(null, "Expense updated successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, null, [ex.Message]));
        }
    }

    /// <summary>
    /// Deletes a specific expense.
    /// </summary>
    /// <param name="id">The ID of the expense to delete.</param>
    /// <returns>Confirmation of deletion.</returns>
    /// <response code="200">Expense deleted successfully.</response>
    /// <response code="404">Expense not found.</response>
    /// <response code="401">Unauthorized access.</response>
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [HttpDelete(ApiRoutes.Expense.Delete.ById)]
    public async Task<IActionResult> DeleteExpenseAsync(Guid id)
    {
        try
        {
            Guid userId = GetCurrentUserId();
            bool result = await _expenseService.DeleteExpenseAsync(id, userId);

            if (!result)
            {
                return NotFound(ApiResponse<object?>.Fail(null, $"Expense with ID {id} not found"));
            }

            return Ok(ApiResponse<object?>.Ok(null, "Expense deleted successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, null, [ex.Message]));
        }
    }

    /// <summary>
    /// Deletes all expenses for the authenticated user.
    /// </summary>
    /// <returns>Confirmation message.</returns>
    /// <response code="200">All expenses deleted successfully.</response>
    /// <response code="404">No expenses found to delete.</response>
    /// <response code="401">Unauthorized access.</response>
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [HttpDelete(ApiRoutes.Expense.Delete.All)]
    public async Task<IActionResult> DeleteAllExpensesAsync()
    {
        try
        {
            Guid userId = GetCurrentUserId();
            bool result = await _expenseService.DeleteAllExpensesAsync(userId);

            if (!result)
            {
                return NotFound(ApiResponse<object?>.Fail(null, "No expenses found to delete."));
            }

            return Ok(ApiResponse<object?>.Ok(null, "All expenses deleted successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, null, [ex.Message]));
        }
    }
}