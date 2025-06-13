using ExpenseTracker.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MetaController : ControllerBase
{
	[HttpGet("get-expense-categories")]
	public IActionResult GetExpenseCategories()
	{
		var categories = Enum.GetValues(typeof(ExpenseCategory))
			.Cast<ExpenseCategory>()
			.Select(c => new
			{
				value = c.ToString(),
				displayName = GetDisplayName(c)
			});

		return Ok(categories);
	}

	private static string GetDisplayName(Enum value)
	{
		return value.GetType()
			.GetMember(value.ToString())[0]
			.GetCustomAttribute<DisplayAttribute>()?.Name ?? value.ToString();
	}
}
