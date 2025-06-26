using System.Security.Claims;
using ExpenseTracker.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/savinggoal")]
public class SavingGoalController : ControllerBase
{
    private readonly ISavingGoalService _savingGoalService;

    public SavingGoalController(ISavingGoalService savingGoalService)
    {
        _savingGoalService = savingGoalService;
    }

    [HttpPost]
    [Route(ExpenseRoutes.SavingGoalPostUrl.Create)]
    public async Task<IActionResult> CreateGoal([FromBody] CreateSavingGoalRequest request)
    {
        var userId = GetUserId();
        var result = await _savingGoalService.CreateGoalAsync(request, userId);

        if (!result.IsSuccess)
            return Conflict(new { message = result.ErrorMessage });

        return Created("", result.Data);
    }

    [HttpGet]
    [Route(ExpenseRoutes.SavingGoalGetUrl.Status)]
    public async Task<IActionResult> GetStatus([FromQuery] int month, [FromQuery] int year)
    {
        var userId = GetUserId();
        var (target, saved, percent) = await _savingGoalService.GetSavingStatusAsync(userId, month, year);

        if (target == 0)
            return NotFound(new { message = "No saving goal found for this period." });

        return Ok(new
        {
            Target = target,
            Saved = saved,
            Progress = $"{percent}%",
            Message = $"You saved ₦{saved:N0} out of ₦{target:N0} ({percent}% completed)."
        });
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim?.Value, out var userId) ? userId : Guid.Empty;
    }
}
