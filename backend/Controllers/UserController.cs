using ExpenseTracker.Models.DTOs.Settings;
using ExpenseTracker.Services;
using ExpenseTracker.Utilities.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ExpenseTracker.Controllers;

/// <summary>
/// User settings and account management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UserController(IAuthService authService, UserSettingsService settingsService, IExchangeRateService exchangeRateService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly UserSettingsService _settingsService = settingsService;
    private readonly IExchangeRateService _exchangeRateService = exchangeRateService;

    /// <summary>
    /// Update user profile (name, email)
    /// </summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(new { success = false, message = "Invalid token." });
        }

        var result = await _authService.UpdateProfileAsync(userId, request.Name, request.Email);

        if (result.Success)
        {
            return Ok(new { success = true, message = result.Message, data = result.Data });
        }

        return BadRequest(new { success = false, message = result.Message });
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(new { success = false, message = "Invalid token." });
        }

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return BadRequest(new { success = false, message = "New passwords do not match." });
        }

        var result = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

        if (result.Success)
        {
            return Ok(new { success = true, message = result.Message });
        }

        return BadRequest(new { success = false, message = result.Message });
    }

    /// <summary>
    /// Get user settings/preferences
    /// </summary>
    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(new { success = false, message = "Invalid token." });
        }

        var settings = await _settingsService.GetSettingsAsync(userId);
        return Ok(new { success = true, data = settings });
    }

    /// <summary>
    /// Update user settings/preferences
    /// </summary>
    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingsRequest request)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(new { success = false, message = "Invalid token." });
        }

        var result = await _settingsService.UpdateSettingsAsync(userId, request);

        if (result.Success)
        {
            return Ok(new { success = true, message = result.Message, data = result.Data });
        }

        return BadRequest(new { success = false, message = result.Message });
    }

    /// <summary>
    /// Get list of supported currencies
    /// </summary>
    [HttpGet("currencies")]
    [AllowAnonymous]
    public IActionResult GetCurrencies()
    {
        var currencies = SupportedCurrencies.Currencies.Select(c => new
        {
            code = c.Key,
            name = c.Value
        });

        return Ok(new { success = true, data = currencies });
    }

    /// <summary>
    /// Get current exchange rates from NGN base
    /// </summary>
    [HttpGet("exchange-rates")]
    [AllowAnonymous]
    public async Task<IActionResult> GetExchangeRates([FromQuery] string baseCurrency = "NGN")
    {
        var rates = await _exchangeRateService.GetExchangeRatesAsync(baseCurrency);
        return Ok(new { success = true, data = new { baseCurrency, rates } });
    }

    /// <summary>
    /// Export all user data as JSON
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> ExportData()
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(new { success = false, message = "Invalid token." });
        }

        var jsonData = await _settingsService.ExportUserDataAsync(userId);
        var bytes = Encoding.UTF8.GetBytes(jsonData);
        var fileName = $"expense-tracker-export-{DateTime.UtcNow:yyyy-MM-dd}.json";

        return File(bytes, "application/json", fileName);
    }

    /// <summary>
    /// Delete user account permanently
    /// </summary>
    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(new { success = false, message = "Invalid token." });
        }

        var result = await _settingsService.DeleteAccountAsync(userId, request.Password, request.SecurityQuestionId, request.SecurityAnswer);

        if (result.Success)
        {
            return Ok(new { success = true, message = result.Message });
        }

        return BadRequest(new { success = false, message = result.Message });
    }
}
