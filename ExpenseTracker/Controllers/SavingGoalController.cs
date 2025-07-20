using System.Security.Claims;
using ExpenseTracker.Contracts;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

/// <summary>
/// Controller for managing saving goals in the expense tracker application.
/// </summary>
/// <param name="savingGoalService"></param>
[ApiController, Authorize]
public class SavingGoalController(ISavingGoalService savingGoalService) : ControllerBase
{
    private readonly ISavingGoalService _savingGoalService = savingGoalService;

    /// <summary>
    /// Creates a new saving goal.
    /// </summary>
    /// <remarks>
    /// This endpoint authenticates a user with their email and password.
    /// On success, it returns a JWT that can be used for future requests.
    /// </remarks>
    /// <param name="request">The data required to create the saving goal.</param>
    /// <returns>The newly created saving goal.</returns>
    /// <response code="201">Returns the created saving goal</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="500">If an unhandled error occurs</response>
    [ProducesResponseType(typeof(ApiResponse<SavingGoalResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Route(ApiRoutes.Savings.Post.Create)]
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
    [Route(ApiRoutes.Savings.Get.All)]
    public async Task<IActionResult> GetAllGoals([FromQuery] bool includeArchived = false)
    {
        Guid userId = GetUserId();
        IEnumerable<SavingGoal> goals = await _savingGoalService.GetAllGoalsAsync(userId, includeArchived);
        return Ok(goals.Select(SavingGoalResponse.MapToResponse));
    }

    /// <summary>
    /// Gets a single saving goal by ID.
    /// </summary>
    [HttpGet(ApiRoutes.Savings.Get.ById)]
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
    [HttpPut(ApiRoutes.Savings.Put.Update)]
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
    [HttpPatch(ApiRoutes.Savings.Patch.Archive)]
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
    [HttpDelete(ApiRoutes.Savings.Delete.ById)]
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
    [Route(ApiRoutes.Savings.Post.Contribute)]
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