namespace ExpenseTracker.Models.DTOs.Dashboards;

/// <summary>
/// Represents the total amount spent by the user on a specific day.
/// Used for rendering daily trends in line charts.
/// </summary>
public class DailySpendingDto
{
	/// <summary>
	/// Gets or sets the date of the expense activity.
	/// </summary>
	public required DateOnly Date { get; set; }

	/// <summary>
	/// Gets or sets the total amount spent on the specified date.
	/// </summary>
	public required double TotalSpent { get; set; }
}
