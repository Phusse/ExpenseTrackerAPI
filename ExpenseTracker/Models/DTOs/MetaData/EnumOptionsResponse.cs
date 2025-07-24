namespace ExpenseTracker.Models.DTOs.Metadata;

/// <summary>
/// Represents an enum value-label pair for frontend usage.
/// </summary>
public class EnumOptionResponse
{
	/// <summary>
	/// The enum value (e.g., "Cash").
	/// </summary>
	public required string Value { get; set; }

	/// <summary>
	/// The human-readable label (e.g., "Cash" or "Food and Groceries").
	/// </summary>
	public required string Label { get; set; }
}
