using System.Net;
using System.Text.Json;
using ExpenseTracker.Models;

namespace ExpenseTracker.Middleware;

/// <summary>
/// Middleware for globally handling unhandled exceptions in the application pipeline.
/// Converts exceptions into structured <see cref="ApiResponse{T}"/> JSON responses.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware in the request pipeline.</param>
public class ExceptionHandlingMiddleware(RequestDelegate next)
{
	private readonly RequestDelegate _next = next;

	/// <summary>
	/// Executes the middleware logic by invoking the next middleware and handling any unhandled exceptions.
	/// </summary>
	/// <param name="context">The current HTTP context.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}

	/// <summary>
	/// Converts the caught exception into a standardized JSON error response using <see cref="ApiResponse{T}"/>.
	/// </summary>
	/// <param name="context">The current HTTP context.</param>
	/// <param name="ex">The exception that was thrown.</param>
	/// <returns>A task representing the asynchronous response writing operation.</returns>
	private static Task HandleExceptionAsync(HttpContext context, Exception ex)
	{
		context.Response.ContentType = "application/json";

		context.Response.StatusCode = ex switch
		{
			ArgumentException => (int)HttpStatusCode.BadRequest,
			KeyNotFoundException => (int)HttpStatusCode.NotFound,
			_ => (int)HttpStatusCode.InternalServerError,
		};

		var response = ApiResponse<object?>.Fail(null, null, [ex.Message]);
		return context.Response.WriteAsync(JsonSerializer.Serialize(response));
	}
}
