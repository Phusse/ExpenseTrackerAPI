namespace ExpenseTracker.Models.DTOs.Dashboards;

/// <summary>
/// Represents a recent individual expense transaction recorded by the user.
/// Used to show a snapshot of latest financial activity.
/// </summary>
public class RecentTransactionDto
{
	/// <summary>
	/// Gets or sets the unique identifier of the transaction.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the expense category.
	/// </summary>
	public required string Category { get; set; }

	/// <summary>
	/// Gets or sets the amount of the transaction.
	/// </summary>
	public required double Amount { get; set; }

	/// <summary>
	/// Gets or sets the date the expense occurred.
	/// </summary>
	public required DateTime? DateOfExpense { get; set; }

	/// <summary>
	/// Gets or sets the optional description or note for the transaction.
	/// </summary>
	public string? Description { get; set; }
}
