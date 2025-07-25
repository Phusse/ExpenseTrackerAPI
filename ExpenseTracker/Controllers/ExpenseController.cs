using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using ExpenseTracker.Utilities.Routing;
using ExpenseTracker.Services;
using ExpenseTracker.Models.DTOs.Expenses;
using ExpenseTracker.Utilities.Extension;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers;

[Authorize]
[ApiController]
public class ExpenseController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _expenseService = expenseService;

    private Guid GetCurrentUserId()
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }

        return userId;
    }

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

    // PUT: api/v1/expense/{id}
    [HttpPut]
    [Route(ApiRoutes.Expense.Put.Update)]
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

    // DELETE: api/v1/expense/{id}
    [HttpDelete]
    [Route(ApiRoutes.Expense.Delete.ById)]
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

    // DELETE: api/v1/expense/all
    [HttpDelete]
    [Route(ApiRoutes.Expense.Delete.All)]
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