namespace ExpenseTracker.Services;

public interface IMetaService
{
	IEnumerable<(string value, string displayName)> GetExpenseCategories();
}
