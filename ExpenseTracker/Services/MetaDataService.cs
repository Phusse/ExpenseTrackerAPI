using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ExpenseTracker.Enums;
using ExpenseTracker.Models.DTOs.Metadata;

namespace ExpenseTracker.Services;

/// <summary>
/// Provides methods to retrieve enum options for metadata, such as expense categories, payment methods, and saving goal statuses,
/// </summary>
internal class MetadataService : IMetadataService
{
	private static List<EnumOptionResponse> GetEnumOptions<T>() where T : Enum
	{
		return [.. Enum.GetValues(typeof(T))
			.Cast<T>()
			.Select(e => new EnumOptionResponse
			{
				Value = e.ToString(),
				Label = e.GetType()
					.GetMember(e.ToString())
					.First()
					.GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString()
			})];
	}

	public List<EnumOptionResponse> GetExpenseCategories() => GetEnumOptions<ExpenseCategory>();
	public List<EnumOptionResponse> GetPaymentMethods() => GetEnumOptions<PaymentMethod>();
	public List<EnumOptionResponse> GetSavingGoalStatuses() => GetEnumOptions<SavingGoalStatus>();
}
