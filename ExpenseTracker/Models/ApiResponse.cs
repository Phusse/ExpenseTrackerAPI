namespace ExpenseTracker.Models;

/// <summary>
/// Standard API response wrapper that provides a consistent structure for all API results.
/// T represents the data payload returned in the response.
/// </summary>
/// <typeparam name="T">The type of the data payload returned in the response.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the API operation was successful.
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// A message describing the outcome of the operation (e.g., success or error message).
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// A list of errors related to the operation, such as validation or business logic errors.
    /// May be null or empty if no errors occurred.
    /// </summary>
    public List<string>? Errors { get; init; }

    /// <summary>
    /// The UTC timestamp indicating when the response was generated.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Optional data returned by the API. May be null for operations that do not return content.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Creates a successful API response.
    /// </summary>
    /// <param name="data">The payload returned from the operation.</param>
    /// <param name="message">An optional success message. Defaults to "Operation successful" if not provided.</param>
    /// <returns>An <see cref="ApiResponse{T}"/> indicating a successful result.</returns>
    public static ApiResponse<T> Success(T? data, string? message = null) => new()
    {
        IsSuccess = true,
        Message = string.IsNullOrWhiteSpace(message) ? "Operation successful" : message,
        Data = data,
        Errors = null,
    };

    /// <summary>
    /// Creates a failed API response.
    /// </summary>
    /// <param name="data">The payload returned from the operation, if any. Can be null.</param>
    /// <param name="message">An optional failure message. Defaults to "Operation failed" if not provided.</param>
    /// <param name="errors">A list of error messages. Can be null or empty.</param>
    /// <returns>An <see cref="ApiResponse{T}"/> indicating a failed result.</returns>
    public static ApiResponse<T> Failure(T? data, string? message = null, List<string>? errors = null) => new()
    {
        IsSuccess = false,
        Message = string.IsNullOrWhiteSpace(message) ? "Operation failed" : message,
        Data = data,
        Errors = errors,
    };
}
