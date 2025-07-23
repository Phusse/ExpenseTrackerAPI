namespace ExpenseTracker.Models;

/// <summary>
/// Standard API response wrapper that provides a consistent structure for all API results.
/// </summary>
/// <typeparam name="T">The type of the data payload returned in the response.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the API operation was successful.
    /// </summary>
    public required bool Success { get; set; }

    /// <summary>
    /// A message describing the outcome of the operation (e.g., success or error message).
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// A list of errors related to the operation, such as validation or warnings.
    /// Can be used for both success and failure responses.
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// The UTC timestamp indicating when the response was generated.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional data returned by the API. May be null for operations that do not return content.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Creates a successful API response.
    /// </summary>
    /// <param name="data">Optional payload returned from the operation.</param>
    /// <param name="message">Optional success message. Defaults to "Operation successful".</param>
    /// <param name="errors">Optional list of non-fatal issues or warnings related to the operation.</param>
    /// <returns>A successful <see cref="ApiResponse{T}"/> instance.</returns>
    public static ApiResponse<T> Ok(T? data = default, string? message = null, List<string>? errors = null) => new()
    {
        Success = true,
        Message = string.IsNullOrWhiteSpace(message) ? "Operation successful." : message,
        Data = data,
        Errors = errors,
    };

    /// <summary>
    /// Creates a failed API response.
    /// </summary>
    /// <param name="data">Optional payload returned from the operation, if any.</param>
    /// <param name="message">Optional failure message. Defaults to "Operation failed".</param>
    /// <param name="errors">A list of error messages explaining the failure.</param>
    /// <returns>A failed <see cref="ApiResponse{T}"/> instance.</returns>
    public static ApiResponse<T> Fail(T? data = default, string? message = null, List<string>? errors = null) => new()
    {
        Success = false,
        Message = string.IsNullOrWhiteSpace(message) ? "Operation failed." : message,
        Data = data,
        Errors = errors,
    };
}
