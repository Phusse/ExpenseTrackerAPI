using System.Security.Claims;
using ExpenseTracker.Contracts;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[ApiController, Authorize]
public class SavingGoalController(ISavingGoalService savingGoalService) : ControllerBase
{
    private readonly ISavingGoalService _savingGoalService = savingGoalService;

    /// <summary>
    /// Creates a new saving goal.
    /// </summary>
    [HttpPost]
    [Route(ExpenseRoutes.SavingGoalPostUrl.Create)]
    public async Task<IActionResult> CreateGoal([FromBody] CreateSavingGoalRequest request)
    {
        Guid userId = GetUserId();

        (
            bool isSuccess,
            SavingGoal? data,
            string? errorMessage
        ) = await _savingGoalService.CreateGoalAsync(request, userId);

        return isSuccess
            ? Created(string.Empty, data)
            : Conflict(new { message = errorMessage });
    }

    /// <summary>
    /// Gets all saving goals for the user.
    /// </summary>
    [HttpGet]
    [Route(ExpenseRoutes.SavingGoalGetUrl.GetAll)]
    public async Task<IActionResult> GetAllGoals([FromQuery] bool includeArchived = false)
    {
        Guid userId = GetUserId();
        IEnumerable<SavingGoal> goals = await _savingGoalService.GetAllGoalsAsync(userId, includeArchived);
        return Ok(goals.Select(SavingGoalResponse.MapToResponse));
    }

    /// <summary>
    /// Gets a single saving goal by ID.
    /// </summary>
    [HttpGet(ExpenseRoutes.SavingGoalGetUrl.GetById)]
    public async Task<IActionResult> GetGoalById([FromRoute] Guid id)
    {
        Guid userId = GetUserId();
        SavingGoal? goal = await _savingGoalService.GetGoalByIdAsync(id, userId);

        return goal is not null
            ? Ok(SavingGoalResponse.MapToResponse(goal))
            : NotFound(new { message = "Saving goal not found." });
    }

    /// <summary>
    /// Updates a saving goal.
    /// </summary>
    [HttpPut(ExpenseRoutes.SavingGoalPutUrl.Update)]
    public async Task<IActionResult> UpdateGoal([FromRoute] Guid id, [FromBody] UpdateSavingGoalRequest request)
    {
        Guid userId = GetUserId();

        (
            bool isSuccess,
            string? errorMessage
        ) = await _savingGoalService.UpdateGoalAsync(id, request, userId);

        return isSuccess
            ? Ok(new { message = "Saving goal updated successfully." })
            : NotFound(new { message = errorMessage });
    }

    /// <summary>
    /// Archives or unarchives a saving goal.
    /// </summary>
    [HttpPatch(ExpenseRoutes.SavingGoalPatchUrl.Archive)]
    public async Task<IActionResult> ArchiveGoal([FromRoute] Guid id, [FromQuery] bool archiveGoal = true)
    {
        Guid userId = GetUserId();

        (
            bool isSuccess,
            string? message
        ) = await _savingGoalService.ArchiveGoalAsync(id, userId, archiveGoal);

        if (!isSuccess)
        {
            return NotFound(new { message });
        }

        return Ok(new { message });
    }

    /// <summary>
    /// Deletes a saving goal permanently.
    /// </summary>
    [HttpDelete(ExpenseRoutes.SavingGoalDeleteUrl.Delete)]
    public async Task<IActionResult> DeleteGoal([FromRoute] Guid id)
    {
        var userId = GetUserId();
        var result = await _savingGoalService.DeleteGoalAsync(id, userId);

        return result.IsSuccess
            ? Ok(new { message = "Saving goal deleted." })
            : NotFound(new { message = result.ErrorMessage });
    }

    /// <summary>
    /// Adds a savings contribution and creates an expense for it.
    /// </summary>
    [HttpPost]
    [Route(ExpenseRoutes.SavingGoalPostUrl.Contribute)]
    public async Task<IActionResult> AddContribution([FromBody] AddSavingContributionRequest request)
    {
        var userId = GetUserId();
        var result = await _savingGoalService.AddSavingContributionAsync(request, userId);

        return result.IsSuccess
            ? Ok(new { message = "Contribution added successfully." })
            : BadRequest(new { message = result.ErrorMessage });
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim?.Value, out var userId) ? userId : Guid.Empty;
    }
}