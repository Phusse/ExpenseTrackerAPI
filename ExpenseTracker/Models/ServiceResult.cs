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
	public required bool IsSuccess { get; init; }

	/// <summary>
	/// A descriptive message indicating the outcome of the operation.
	/// Used to convey error messages, status updates, or informational text.
	/// </summary>
	public string? Message { get; init; }

	/// <summary>
	/// The result data returned from the operation, if available.
	/// May be <c>null</c> for failures or operations that don't produce a result.
	/// </summary>
	public T? Data { get; init; }

	/// <summary>
	/// Creates a successful <see cref="ServiceResult{T}"/> with the specified result data and optional message.
	/// </summary>
	/// <param name="data">The result data returned by the operation.</param>
	/// <param name="message">An optional message describing the success. Defaults to <c>null</c>.</param>
	/// <returns>A new <see cref="ServiceResult{T}"/> representing a successful result.</returns>
	public static ServiceResult<T> Success(T data, string? message = null) => new()
	{
		IsSuccess = true,
		Message = message,
		Data = data,
	};

	/// <summary>
	/// Creates a failed <see cref="ServiceResult{T}"/> with optional result data and an error message.
	/// </summary>
	/// <param name="data">Optional data associated with the failure. May be <c>null</c>.</param>
	/// <param name="errorMessage">An optional error message describing the failure. Defaults to <c>null</c>.</param>
	/// <returns>A new <see cref="ServiceResult{T}"/> representing a failed result.</returns>
	public static ServiceResult<T> Failure(T data, string? errorMessage = null) => new()
	{
		IsSuccess = false,
		Message = errorMessage,
		Data = data,
	};
}
