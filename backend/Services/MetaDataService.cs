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
	private static readonly List<EnumOptionResponse> ExpenseCategoriesCache = BuildEnumOptions<ExpenseCategory>();
	private static readonly List<EnumOptionResponse> PaymentMethodsCache = BuildEnumOptions<PaymentMethod>();
	private static readonly List<EnumOptionResponse> SavingGoalStatusesCache = BuildEnumOptions<SavingGoalStatus>();

	private static List<EnumOptionResponse> BuildEnumOptions<T>() where T : Enum
	{
		Type type = typeof(T);
		Array values = Enum.GetValues(type);
		List<EnumOptionResponse> result = new(values.Length);

		foreach (object value in values)
		{
			string name = value.ToString()!;

			string label = type
				.GetMember(name)[0]
				.GetCustomAttribute<DisplayAttribute>()?.Name ?? name;

			result.Add(new EnumOptionResponse
			{
				Value = name,
				Label = label
			});
		}

		return result;
	}

	public List<EnumOptionResponse> GetExpenseCategories() => ExpenseCategoriesCache;
	public List<EnumOptionResponse> GetPaymentMethods() => PaymentMethodsCache;
	public List<EnumOptionResponse> GetSavingGoalStatuses() => SavingGoalStatusesCache;
}

