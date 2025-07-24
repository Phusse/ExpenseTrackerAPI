using ExpenseTracker.Models.DTOs.Metadata;

namespace ExpenseTracker.Services;

/// <summary>
/// Defines methods for retrieving enum values and their display names.
/// </summary>
public interface IMetadataService
{
	/// <summary>
	/// Gets all expense categories and their display names.
	/// </summary>
	List<EnumOptionResponse> GetExpenseCategories();

	/// <summary>
	/// Gets all payment methods and their display names.
	/// </summary>
	List<EnumOptionResponse> GetPaymentMethods();

	/// <summary>
	/// Gets all saving goal statuses and their display names.
	/// </summary>
	List<EnumOptionResponse> GetSavingGoalStatuses();
}
