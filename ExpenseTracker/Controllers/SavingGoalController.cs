using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.SavingGoals;
using ExpenseTracker.Services;
using ExpenseTracker.Utilities.Extension;
using ExpenseTracker.Utilities.Routing;
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
    /// This endpoint requires the user to be authenticated with a valid token.
    /// <c>On success</c>, it returns the created saving goal and sets the Location header
    /// to the endpoint where the goal can be retrieved.
    /// </remarks>
    /// <param name="request">The data required to create the saving goal.</param>
    /// <returns>The newly created saving goal wrapped in a standard API response.</returns>
    /// <response code="201">Returns the created saving goal</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="409">If the saving goal could not be created due to a conflict</response>
    /// <response code="500">If an unhandled server error occurs</response>
    [ProducesResponseType(typeof(ApiResponse<CreateSavingGoalResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status500InternalServerError)]
    [HttpPost(ApiRoutes.Savings.Post.Create)]
    public async Task<IActionResult> CreateGoal([FromBody] CreateSavingGoalRequest request)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing token."));
        }

        ServiceResult<CreateSavingGoalResponse?> result = await _savingGoalService.CreateGoalAsync(request, userId);

        if (result.Success)
        {
            return CreatedAtAction(
                nameof(GetGoalById),
                new { id = result.Data!.Id },
                ApiResponse<CreateSavingGoalResponse?>.Ok(result.Data, result.Message)
            );
        }

        return Conflict(ApiResponse<object?>.Fail(null, result.Message ?? "Saving goal creation failed."));
    }

    /// <summary>
    /// Retrieves all saving goals for the authenticated user.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of the user's saving goals.
    /// If <paramref name="includeArchived"/> is <c>true</c>, archived goals will also be included.
    /// </remarks>
    /// <param name="includeArchived">If <c>true</c>, includes archived saving goals. Default is <c>false</c>.</param>
    /// <returns>
    /// A list of saving goals wrapped in a standard API response.
    /// </returns>
    /// <response code="200">Returns the list of saving goals</response>
    /// <response code="401">If the user is not authenticated</response>
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CreateSavingGoalResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route(ApiRoutes.Savings.Get.All)]
    public async Task<IActionResult> GetAllGoals([FromQuery] bool includeArchived = false)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing user token."));
        }

        IEnumerable<CreateSavingGoalResponse> goals = await _savingGoalService.GetAllGoalsAsync(userId, includeArchived);

        return Ok(ApiResponse<IEnumerable<CreateSavingGoalResponse>>.Ok(
            goals,
            "Successfully retrieved all saving goals."
        ));
    }

    /// <summary>
    /// Retrieves a specific saving goal for the authenticated user by its unique ID.
    /// </summary>
    /// <remarks>
    /// Requires the user to be authenticated with a valid token. If the saving goal is not found
    /// or does not belong to the user, a 404 response is returned.
    /// </remarks>
    /// <param name="id">The unique identifier of the saving goal to retrieve.</param>
    /// <returns>
    /// The requested saving goal wrapped in a standard API response.
    /// </returns>
    /// <response code="200">Returns the saving goal</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the saving goal does not exist or is not owned by the user</response>
    [ProducesResponseType(typeof(ApiResponse<CreateSavingGoalResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [HttpGet(ApiRoutes.Savings.Get.ById)]
    public async Task<IActionResult> GetGoalById([FromRoute] Guid id)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing user token."));
        }

        CreateSavingGoalResponse? goal = await _savingGoalService.GetGoalByIdAsync(id, userId);

        if (goal is null)
        {
            return NotFound(ApiResponse<object?>.Fail(null, "Saving goal not found."));
        }

        return Ok(ApiResponse<CreateSavingGoalResponse>.Ok(goal, "Successfully retrieved saving goal."));
    }

    /// <summary>
    /// Updates an existing saving goal for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires the user to be authenticated with a valid token.
    /// If the saving goal is not found or does not belong to the user, a 404 response is returned.
    /// </remarks>
    /// <param name="id">The ID of the saving goal to update.</param>
    /// <param name="request">The updated saving goal data.</param>
    /// <returns>
    /// An API response indicating the result of the operation.
    /// </returns>
    /// <response code="200">If the goal was successfully updated</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the goal was not found or could not be updated</response>
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [HttpPut(ApiRoutes.Savings.Put.Update)]
    public async Task<IActionResult> UpdateGoal([FromRoute] Guid id, [FromBody] UpdateSavingGoalRequest request)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing user token."));
        }

        ServiceResult<object?> result = await _savingGoalService.UpdateGoalAsync(id, request, userId);

        return result.Success
            ? Ok(ApiResponse<object?>.Ok(null, "Saving goal updated successfully."))
            : NotFound(ApiResponse<object?>.Fail(null, result.Message ?? "Failed to update saving goal."));
    }

    /// <summary>
    /// Archives or unarchives a saving goal for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires the user to be authenticated. If the saving goal is not found, a 404 response is returned.
    /// </remarks>
    /// <param name="id">The ID of the saving goal to archive or unarchive.</param>
    /// <param name="archiveGoal">
    /// If <c>true</c>, archives the goal; if <c>false</c>, unarchives it. Default is <c>true</c>.
    /// </param>
    /// <returns>
    /// An API response indicating whether the operation succeeded.
    /// </returns>
    /// <response code="200">If the goal was successfully archived or unarchived</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the saving goal was not found</response>
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [HttpPatch(ApiRoutes.Savings.Patch.Archive)]
    public async Task<IActionResult> ArchiveGoal([FromRoute] Guid id, [FromQuery] bool archiveGoal = true)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing user token."));
        }

        ServiceResult<object?> result = await _savingGoalService.ArchiveGoalAsync(id, userId, archiveGoal);

        return result.Success
            ? Ok(ApiResponse<object?>.Ok(null, result.Message ?? "Goal archive state updated."))
            : NotFound(ApiResponse<object?>.Fail(null, result.Message ?? "Saving goal not found."));
    }

    /// <summary>
    /// Permanently deletes a saving goal for the authenticated user.
    /// </summary>
    /// <remarks>
    /// This operation cannot be undone. Requires authentication via a valid token.
    /// </remarks>
    /// <param name="id">The ID of the saving goal to delete.</param>
    /// <returns>
    /// A response indicating success or failure of the delete operation.
    /// </returns>
    /// <response code="200">If the saving goal was successfully deleted</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the saving goal was not found</response>
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [HttpDelete(ApiRoutes.Savings.Delete.ById)]
    public async Task<IActionResult> DeleteGoal([FromRoute] Guid id)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing user token."));
        }

        var result = await _savingGoalService.DeleteGoalAsync(id, userId);

        return result.Success
            ? Ok(ApiResponse<object?>.Ok(null, result.Message ?? "Saving goal deleted."))
            : NotFound(ApiResponse<object?>.Fail(null, result.Message ?? "Saving goal not found."));
    }

    /// <summary>
    /// Adds a contribution to a saving goal and automatically creates a related expense.
    /// </summary>
    /// <remarks>
    /// The user must be authenticated via a valid token. The contribution amount is added to the target goal,
    /// and an expense is recorded under the relevant category.
    /// </remarks>
    /// <param name="request">The contribution details, including goal ID and amount.</param>
    /// <returns>
    /// A response indicating success or failure of the contribution operation.
    /// </returns>
    /// <response code="200">If the contribution was successfully added</response>
    /// <response code="400">If the request is invalid or the operation failed</response>
    /// <response code="401">If the user is not authenticated</response>
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [HttpPost]
    [Route(ApiRoutes.Savings.Post.Contribute)]
    public async Task<IActionResult> AddContribution([FromBody] AddSavingContributionRequest request)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing user token."));
        }

        var result = await _savingGoalService.AddSavingContributionAsync(request, userId);

        return result.Success
            ? Ok(ApiResponse<object?>.Ok(null, result.Message ?? "Contribution added successfully."))
            : BadRequest(ApiResponse<object?>.Fail(null, result.Message ?? "Failed to add contribution."));
    }
}