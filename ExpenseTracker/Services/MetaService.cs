using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ExpenseTracker.Contracts.Services;
using ExpenseTracker.Core.Enums;

namespace ExpenseTracker.Services;

public class MetaService : IMetaService
{
	public IEnumerable<(string value, string displayName)> GetExpenseCategories()
	{
		return Enum.GetValues(typeof(ExpenseCategory))
			.Cast<ExpenseCategory>()
			.Select(c => (c.ToString(), GetDisplayName(c)));
	}

	private static string GetDisplayName(Enum value)
	{
		return value.GetType()
			.GetMember(value.ToString())[0]
			.GetCustomAttribute<DisplayAttribute>()?.Name ?? value.ToString();
	}
}
