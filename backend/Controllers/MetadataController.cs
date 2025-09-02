using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Metadata;
using ExpenseTracker.Services;
using ExpenseTracker.Utilities.Routing;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

/// <summary>
/// Provides endpoints to retrieve enum values used across the application.
/// </summary>
[ApiController]
public class MetadataController(IMetadataService enumService) : ControllerBase
{
	private readonly IMetadataService _enumService = enumService;

	/// <summary>
	/// Returns all expense categories.
	/// </summary>
	/// <remarks>
	/// Retrives the values if the <c>ExpenseCategory</c> enum, including
	/// their display names
	/// </remarks>
	/// <response code="200">Expense categories retrieved successfully.</response>
	[HttpGet(ApiRoutes.Metadata.Get.ExpenseCategories)]
	[ProducesResponseType(typeof(ApiResponse<List<EnumOptionResponse>>), StatusCodes.Status200OK)]
	public ActionResult<ApiResponse<List<EnumOptionResponse>>> GetExpenseCategories()
	{
		return Ok(ApiResponse<List<EnumOptionResponse>>.Ok(_enumService.GetExpenseCategories()));
	}

	/// <summary>
	/// Returns all payment methods.
	/// </summary>
	/// <remarks>
	/// Retrives the values if the <c>PaymentMethod</c> enum, including
	/// their display names
	/// </remarks>
	/// <response code="200">Payment methods retrieved successfully.</response>
	[HttpGet(ApiRoutes.Metadata.Get.PaymentMethods)]
	[ProducesResponseType(typeof(ApiResponse<List<EnumOptionResponse>>), StatusCodes.Status200OK)]
	public ActionResult<ApiResponse<List<EnumOptionResponse>>> GetPaymentMethods()
	{
		return Ok(ApiResponse<List<EnumOptionResponse>>.Ok(_enumService.GetPaymentMethods()));
	}

	/// <summary>
	/// Returns all saving goal statuses.
	/// </summary>
	/// <remarks>
	/// Retrives the values if the <c>SavingGoalStatus</c> enum, including
	/// their display names
	/// </remarks>
	/// <response code="200">Saving goal statuses retrieved successfully.</response>
	[HttpGet(ApiRoutes.Metadata.Get.SavingGoalStatuses)]
	[ProducesResponseType(typeof(ApiResponse<List<EnumOptionResponse>>), StatusCodes.Status200OK)]
	public ActionResult<ApiResponse<List<EnumOptionResponse>>> GetSavingGoalStatuses()
	{
		return Ok(ApiResponse<List<EnumOptionResponse>>.Ok(_enumService.GetSavingGoalStatuses()));
	}
}
