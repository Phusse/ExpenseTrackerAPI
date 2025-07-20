using System.Security.Claims;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Contracts;
using System.Globalization;
using ExpenseTracker.Models.Responses;

namespace ExpenseTracker.Controllers;

[ApiController]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }
        return userId;
    }

    // POST: api/v1/expense
    [HttpPost]
    [Route(ApiRoutes.Expense.Post.Create)]
    public async Task<IActionResult> CreateExpense([FromBody] Expense expense)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _expenseService.CreateExpenseAsync(expense, userId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, new ApiResponse<Expense>
                {
                    Success = false,
                    Message = result.Message ?? "An error occurred",
                    Data = null
                });
            }

            return CreatedAtAction(nameof(CreateExpense), new ApiResponse<Expense>
            {
                Success = true,
                Message = result.Message, // ✅ Use the budget summary message here
                Data = result.Data        // ✅ Already has user stripped
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<Expense>
            {
                Success = false,
                Message = "User not authorized.",
                Data = null
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<Expense>
            {
                Success = false,
                Message = $"Unexpected error: {ex.Message}",
                Data = null
            });
        }
    }

    // GET: api/v1/expense/{id}
    [HttpGet]
    [Route(ApiRoutes.Expense.Get.ById)]
    public async Task<ActionResult<Expense>> GetExpenseByIdAsync(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            Expense? expense = await _expenseService.GetExpenseByIdAsync(id, userId);

            if (expense == null)
            {
                return NotFound($"Expense with Id {id} not found.");
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
    public async Task<ActionResult<IEnumerable<Expense>>> GetAllExpensesAsync()
    {
        try
        {
            var userId = GetCurrentUserId();
            IEnumerable<Expense> expenses = await _expenseService.GetAllExpensesAsync(userId);
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
            var expenses = await _expenseService.GetFilteredExpensesAsync(
                userId, startDate, endDate, minAmount, maxAmount, exactAmount, category);

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
            Console.WriteLine($"Error filtering expenses: {ex.Message}");
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

            // Validate month
            if (month.HasValue && (month < 1 || month > 12))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Month must be between 1 and 12."
                });
            }

            // Validate year
            if (year.HasValue && year < 1)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Year must be a valid positive number."
                });
            }

            var total = await _expenseService.GetTotalExpenseAsync(userId, startDate, endDate, month, year);

            // Dynamic message
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

            return Ok(new
            {
                success = true,
                message,
                totalExpense = total
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
                message = "An error occurred while calculating total expense.",
                error = ex.Message
            });
        }
    }

    // PUT: api/v1/expense/{id}
    [HttpPut]
    [Route(ApiRoutes.Expense.Put.Update)]
    public async Task<ActionResult> UpdateExpenseAsync(Guid id, [FromBody] Expense expenseToUpdate)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (expenseToUpdate == null || id != expenseToUpdate.Id)
            {
                return BadRequest(new { success = false, message = "Invalid data or ID mismatch." });
            }

            var success = await _expenseService.UpdateExpenseAsync(id, expenseToUpdate, userId);

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
    public async Task<ActionResult> DeleteExpenseAsync(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            bool deleteSuccessful = await _expenseService.DeleteExpenseAsync(id, userId);

            if (!deleteSuccessful)
            {
                return NotFound($"Expense with Id {id} not found.");
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
    public async Task<ActionResult> DeleteAllExpensesAsync()
    {
        try
        {
            var userId = GetCurrentUserId();
            bool deleteSuccessful = await _expenseService.DeleteAllExpensesAsync(userId);

            if (!deleteSuccessful)
            {
                return NotFound($"No expenses found to delete.");
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}