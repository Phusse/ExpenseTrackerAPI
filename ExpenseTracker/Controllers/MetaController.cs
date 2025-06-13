using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MetaController(IMetaService metaService) : ControllerBase
{
	private readonly IMetaService _metaService = metaService;

	[HttpGet("get-expense-categories")]
	public IActionResult GetExpenseCategories()
	{
		var categories = _metaService.GetExpenseCategories()
			.Select(c => new { c.value, c.displayName });

		return Ok(categories);
	}
}
