using System.Security.Claims;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using ExpenseTracker.Utilities.Routing;
using ExpenseTracker.Services;
using ExpenseTracker.Models.DTOs.Expenses;
using ExpenseTracker.Utilities.Extension;

namespace ExpenseTracker.Controllers;

[ApiController]
[Authorize]
public class ExpenseController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _expenseService = expenseService;

    private Guid GetCurrentUserId()
    {
        if (!User.TryGetUserId(out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }
        return userId;
    }

    // POST: api/v1/expense
    [HttpPost]
    [Route(ApiRoutes.Expense.Post.Create)]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _expenseService.CreateExpenseAsync(userId, request);

            return CreatedAtAction(nameof(CreateExpense), result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Unexpected error: {ex.Message}"
            });
        }
    }

    // GET: api/v1/expense/{id}
    [HttpGet]
    [Route(ApiRoutes.Expense.Get.ById)]
    public async Task<IActionResult> GetExpenseByIdAsync(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var expense = await _expenseService.GetExpenseByIdAsync(id, userId);

            if (expense == null)
            {
                return NotFound();
            }

            return Ok(expense);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    // GET: api/v1/expense/getall
    [HttpGet]
    [Route(ApiRoutes.Expense.Get.All)]
    public async Task<IActionResult> GetAllExpensesAsync()
    {
        try
        {
            var userId = GetCurrentUserId();
            var expenses = await _expenseService.GetAllExpensesAsync(userId);
            return Ok(expenses);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpGet]
    [Route(ApiRoutes.Expense.Get.Filter)]
    public async Task<IActionResult> GetFilteredExpenses(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount,
        [FromQuery] decimal? exactAmount,
        [FromQuery] string? category)
    {
        try
        {
            var userId = GetCurrentUserId();

            var request = new FilteredExpenseRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
                ExactAmount = exactAmount,
                Category = category
            };

            var expenses = await _expenseService.GetFilteredExpensesAsync(userId, request);

            if (!expenses.Any())
            {
                return NotFound(new
                {
                    success = false,
                    message = "No matching expenses found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Filtered expenses retrieved successfully.",
                data = expenses
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving expenses.",
                error = ex.Message
            });
        }
    }

    [HttpGet(ApiRoutes.Expense.Get.Total)]
    public async Task<IActionResult> GetTotalExpense(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int? month,
        [FromQuery] int? year)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (month.HasValue && (month < 1 || month > 12))
            {
                return BadRequest(new { success = false, message = "Month must be between 1 and 12." });
            }

            if (year.HasValue && year < 1)
            {
                return BadRequest(new { success = false, message = "Year must be a valid positive number." });
            }

            var request = new TotalExpenseRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                Month = month,
                Year = year
            };

            var total = await _expenseService.GetTotalExpenseAsync(userId, request);

            string message;
            if (month.HasValue && year.HasValue)
            {
                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Value);
                message = $"Total expense for {monthName} {year} is: {total}";
            }
            else if (startDate.HasValue || endDate.HasValue)
            {
                message = $"Total expense from {startDate?.ToString("yyyy-MM-dd") ?? "beginning"} to {endDate?.ToString("yyyy-MM-dd") ?? "now"} is: {total}";
            }
            else if (year.HasValue)
            {
                message = $"Total expense for the year {year} is: {total}";
            }
            else if (month.HasValue)
            {
                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Value);
                message = $"Total expense for {monthName} is: ${total}";
            }
            else
            {
                message = $"Total expense is: {total}";
            }

            return Ok(new { success = true, message, totalExpense = total });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while calculating total expense.", error = ex.Message });
        }
    }

    // PUT: api/v1/expense/{id}
    [HttpPut]
    [Route(ApiRoutes.Expense.Put.Update)]
    public async Task<IActionResult> UpdateExpenseAsync(Guid id, [FromBody] UpdateExpenseRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (request == null || id != request.Id)
            {
                return BadRequest(new { success = false, message = "Invalid data or ID mismatch." });
            }

            var success = await _expenseService.UpdateExpenseAsync(userId, request);

            if (!success)
            {
                return NotFound(new { success = false, message = $"Expense with ID {id} not found." });
            }

            return Ok(new { success = true, message = "Expense updated successfully." });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    // DELETE: api/v1/expense/{id}
    [HttpDelete]
    [Route(ApiRoutes.Expense.Delete.ById)]
    public async Task<IActionResult> DeleteExpenseAsync(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            bool deleted = await _expenseService.DeleteExpenseAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new { success = false, message = $"Expense with Id {id} not found." });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    // DELETE: api/v1/expense/all
    [HttpDelete]
    [Route(ApiRoutes.Expense.Delete.All)]
    public async Task<IActionResult> DeleteAllExpensesAsync()
    {
        try
        {
            var userId = GetCurrentUserId();
            bool deleted = await _expenseService.DeleteAllExpensesAsync(userId);

            if (!deleted)
            {
                return NotFound(new { success = false, message = "No expenses found to delete." });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}