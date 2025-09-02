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
    /// <remarks>
    /// Accepts an expense payload including amount, date, category, and description.
    /// On success, returns the created expense.
    /// A <c>401 response</c> is returned if the user's identity cannot be resolved.
    /// Any unhandled exception returns a <c>500 error</c>.
    /// </remarks>
    /// <param name="request">The expense data to be recorded.</param>
    /// <response code="201">Expense created successfully.</response>
    /// <response code="401">Unauthorized access due to invalid or missing token.</response>
    /// <response code="500">An internal error occurred while processing the request.</response>
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
    /// Retrieves all expenses recorded by the authenticated user.
    /// </summary>
    /// <remarks>
    /// Returns a complete list of expenses.
    /// If none exist, a 404 is returned with an empty result.
    /// </remarks>
    /// <returns>
    /// An <see cref="IActionResult"/> containing a list of <see cref="CreateExpenseResponse"/> wrapped in an <see cref="ApiResponse{T}"/>.
    /// </returns>
    /// <response code="200">Expenses retrieved successfully.</response>
    /// <response code="404">No expenses found.</response>
    /// <response code="401">Unauthorized access.</response>

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
    /// Retrieves all expenses recorded by the authenticated user.
    /// </summary>
    /// <remarks>
    /// Returns a complete list of expenses.
    /// If none exist, a 404 is returned with an empty result.
    /// </remarks>
    /// <returns>
    /// An <see cref="IActionResult"/> containing a list of <see cref="CreateExpenseResponse"/> wrapped in an <see cref="ApiResponse{T}"/>.
    /// </returns>
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
    /// Retrieves expenses based on filters such as date range or category.
    /// </summary>
    /// <remarks>
    /// Supports filtering by start date, end date, category, and other user-defined parameters.
    /// If no records match the filter, a 404 is returned.
    /// Authentication is required.
    /// </remarks>
    /// <param name="request">The filter criteria for narrowing down expenses.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the filtered list of <see cref="CreateExpenseResponse"/> in an <see cref="ApiResponse{T}"/>.
    /// </returns>
    /// <response code="200">Filtered expenses retrieved successfully.</response>
    /// <response code="404">No expenses match the provided criteria.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">An internal error occurred during processing.</response>
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
    /// Calculates the total amount spent within a specified period or date range.
    /// </summary>
    /// <remarks>
    /// Accepts optional filtering by month (period) or custom date range.
    /// Returns the total sum of matching expenses.
    /// Handles unauthorized or internal error responses accordingly.
    /// </remarks>
    /// <param name="request">Filter parameters to calculate the total.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the total expense as a <see cref="double"/> in an <see cref="ApiResponse{T}"/>.
    /// </returns>
    /// <response code="200">Total expense calculated successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">An internal error occurred while calculating the total.</response>
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
    /// Updates an existing expense for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Matches the provided ID with the request body and updates the record accordingly.
    /// If the expense doesn't exist or the IDs mismatch, appropriate error codes are returned.
    /// </remarks>
    /// <param name="id">The ID of the expense to update.</param>
    /// <param name="request">The updated expense data.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating the outcome of the update operation.
    /// </returns>
    /// <response code="200">Expense updated successfully.</response>
    /// <response code="400">Request data is invalid or ID does not match the request body.</response>
    /// <response code="404">Expense with the specified ID not found.</response>
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
    /// Deletes a specific expense based on its unique ID.
    /// </summary>
    /// <remarks>
    /// Removes the specified expense entry if it belongs to the authenticated user.
    /// A <c>404 response</c> is returned if the record is not found.
    /// </remarks>
    /// <param name="id">The unique identifier of the expense to delete.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating success or failure of the deletion.
    /// </returns>
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
    /// Deletes all expenses for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Removes every expense associated with the user account.
    /// If no expenses exist, a <c>404 reponse</c> is returned.
    /// This action is irreversible.
    /// </remarks>
    /// <returns>
    /// An <see cref="IActionResult"/> confirming deletion status.
    /// </returns>
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
