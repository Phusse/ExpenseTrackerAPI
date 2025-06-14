using ExpenseTracker.Contracts;
using ExpenseTracker.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/" + ApiRoute.Meta.Base)]
public class MetaController(IMetaService metaService) : ControllerBase
{
	private readonly IMetaService _metaService = metaService;

	[HttpGet(ApiRoute.Meta.GetExpenseCategories)]
	public IActionResult GetExpenseCategories()
	{
		var categories = _metaService.GetExpenseCategories()
			.Select(c => new { c.value, c.displayName });

		return Ok(categories);
	}
}
