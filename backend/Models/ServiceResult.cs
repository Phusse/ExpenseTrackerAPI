namespace ExpenseTracker.Models;

/// <summary>
/// Represents the outcome of a service-layer operation, including status, optional result data, and a descriptive message.
/// </summary>
/// <typeparam name="T">The type of data returned by the operation, if any.</typeparam>
public class ServiceResult<T>
{
	/// <summary>
	/// Indicates whether the operation completed successfully.
	/// </summary>
	public required bool Success { get; set; }

	/// <summary>
	/// A descriptive message indicating the outcome of the operation.
	/// Used to convey error messages, status updates, or informational text.
	/// </summary>
	public string? Message { get; set; }

	/// <summary>
	/// A list of errors related to the operation, such as validation or warnings.
	/// Can be used for both success and failure responses.
	/// </summary>
	public List<string>? Errors { get; set; }

	/// <summary>
	/// The result data returned from the operation, if available.
	/// May be <c>null</c> for failures or operations that don't produce a result.
	/// </summary>
	public T? Data { get; set; }

	/// <summary>
	/// Creates a successful <see cref="ServiceResult{T}"/> with the specified result data and optional message.
	/// </summary>
	/// <param name="data">The result data returned by the operation.</param>
	/// <param name="message">An optional message describing the success. Defaults to <c>null</c>.</param>
	/// <param name="errors">Optional list of non-fatal issues or warnings related to the operation.</param>
	/// <returns>A new <see cref="ServiceResult{T}"/> representing a successful result.</returns>
	public static ServiceResult<T> Ok(T data, string? message = null, List<string>? errors = null) => new()
	{
		Success = true,
		Message = message,
		Errors = errors,
		Data = data,
	};

	/// <summary>
	/// Creates a failed <see cref="ServiceResult{T}"/> with optional result data and an error message.
	/// </summary>
	/// <param name="data">Optional data associated with the failure. May be <c>null</c>.</param>
	/// <param name="message">An optional error message describing the failure. Defaults to <c>null</c>.</param>
	/// <param name="errors">A list of error messages explaining the failure.</param>
	/// <returns>A new <see cref="ServiceResult{T}"/> representing a failed result.</returns>
	public static ServiceResult<T> Fail(T data, string? message = null, List<string>? errors = null) => new()
	{
		Success = false,
		Message = message,
		Errors = errors,
		Data = data,
	};
}
