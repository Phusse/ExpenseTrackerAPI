using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace ExpenseTracker.Core.Configurations;

public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _provider = provider;

	public void Configure(SwaggerGenOptions options)
	{
		foreach (var description in _provider.ApiVersionDescriptions)
		{
			options.SwaggerDoc(description.GroupName, new OpenApiInfo
			{
				Title = $"Expense Tracker API {description.ApiVersion}",
				Version = description.ApiVersion.ToString(),
				Description = description.IsDeprecated ? "This API version has been deprecated." : ""
			});
		}
	}
}
