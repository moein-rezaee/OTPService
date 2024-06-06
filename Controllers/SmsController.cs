using CustomResponse.Models;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using OTPService.Common;
using OTPService.DTOs;
using OTPService.Services;

namespace OTPService.Controllers;

[ApiController]
[Route("[action]")]
public class SmsController(
    IDistributedCache cache,
    IConfiguration configuration,
    IValidator<SendCodeDto> sendCodeValidator,
    IValidator<VerifyCodeDto> verifyCodeValidator,
    IHttpClientFactory httpClientFactory,
    ILogger<SmsController> logger) : ControllerBase
{
    private readonly IDistributedCache _cache = cache;
    private readonly ILogger<SmsController> _logger = logger;
    private readonly IValidator<SendCodeDto> _sendCodeValidator = sendCodeValidator;
    private readonly IValidator<VerifyCodeDto> _verifyCodeValidator = verifyCodeValidator;
    private readonly SmsService _service = new(httpClientFactory, configuration);

    [HttpPost]
    public async Task<IActionResult> SendCode(SendCodeDto dto)
    {
        Result result = new();

        try
        {
            //Validation
            var check = _sendCodeValidator.Validate(dto);
            if (!check.IsValid)
            {
                result = CustomErrors.InvalidMobileNumber();
                return StatusCode(result.StatusCode, result);
            }
            //TODO: Check if is develop mode comment
            result = await _service.SendCode(dto);
            string? code = result.Data?.ToString() ?? null;
            if (result.Status && !string.IsNullOrEmpty(code))
            {
                await _cache.SetStringAsync($"{dto.OrganizationId}_{dto.Mobile}", code, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });

                // FIXME: Uncomment
                //TODO: Check if is develop mode uncomment
                // result.Data = null;
            }

            return StatusCode(result.StatusCode, result);
        }
        catch (Exception e)
        {
            _logger.LogInformation(e.Message);
            result = CustomErrors.SendCodeServerError();
            return StatusCode(result.StatusCode, result);
        }
    }

    [HttpPost]
    public async Task<IActionResult> VerifyCode(VerifyCodeDto dto)
    {
        Result result = new();

        try
        {
            string? ValidCode = await _cache.GetStringAsync($"{dto.OrganizationId}_{dto.Mobile}");
            if (ValidCode.IsNullOrEmpty())
            {
                result = CustomErrors.InvalidCode();
                return StatusCode(result.StatusCode, result);
            }

            ValidationResult check = _verifyCodeValidator.Validate(dto);
            if (check.IsValid)
            {
                result = CustomResults.ValidCode();
            }
            else
            {
                result = CustomErrors.InvalidCode();
            }

            return StatusCode(result.StatusCode, result);
        }
        catch (Exception e)
        {
            _logger.LogInformation(e.Message);
            result = CustomErrors.VerifyCodeServerError();
            return StatusCode(result.StatusCode, result);
        }
    }
}
