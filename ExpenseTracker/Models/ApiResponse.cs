namespace ExpenseTracker.Models;

/// <summary>
/// Standard API response wrapper that provides a consistent structure for all API results.
/// T represents the data payload returned in the response.
/// </summary>
/// <typeparam name="T">The type of the data payload returned in the response.</typeparam>
internal class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the API operation was successful.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// A message describing the outcome of the operation (e.g., success or error message).
    /// </summary>
    public required string Message { get; init; }

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
    public static ApiResponse<T> SuccessResponse(T? data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = string.IsNullOrWhiteSpace(message) ? "Operation successful" : message,
            Data = data,
        };
    }

    /// <summary>
    /// Creates a failed API response.
    /// </summary>
    /// <param name="data">The payload returned from the operation, if any. Can be null.</param>
    /// <param name="message">An optional failure message. Defaults to "Operation failed" if not provided.</param>
    /// <returns>An <see cref="ApiResponse{T}"/> indicating a failed result.</returns>
    public static ApiResponse<T> FailureResponse(T? data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = string.IsNullOrWhiteSpace(message) ? "Operation failed" : message,
            Data = data,
        };
    }
}
