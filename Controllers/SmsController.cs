using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OTPService.DTOs;
using OTPService.Services;
using CacheExtension.Abstractions;

namespace OTPService.Controllers;

[ApiController]
[Route("[action]")]
public class SmsController : ControllerBase
{
    private readonly ILogger<SmsController> _logger;
    private readonly IValidator<SendCodeDto> _sendCodeValidator;
    private readonly IValidator<VerifyCodeDto> _verifyCodeValidator;
    private readonly SmsService _service;
    private readonly ICacheService _cache;



    public SmsController(
        IValidator<SendCodeDto> sendCodeValidator,
        IValidator<VerifyCodeDto> verifyCodeValidator,
        SmsService service,
        ICacheService cache,
        ILogger<SmsController> logger)
    {
        _logger = logger;
        _sendCodeValidator = sendCodeValidator;
        _verifyCodeValidator = verifyCodeValidator;
        _service = service;
        _cache = cache;

    }

    [HttpGet("{mobile}")]
    public async Task<IActionResult> SendCode(string mobile)
    {
        var dto = new SendCodeDto()
        {
            Mobile = mobile,
        };

        try
        {
            // Validation via FluentValidation
            _sendCodeValidator.ValidateAndThrow(dto);

            var code = await _service.SendCode(dto);
            if (!string.IsNullOrEmpty(code))
                await _cache.SetAsync($"otp:{mobile}", code, TimeSpan.FromMinutes(2));

            return Ok(new { message = "Code sent" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SendCode failed for {Mobile}", mobile);
            throw;
        }
    }

    [HttpGet("{mobile}/{code}")]
    public async Task<IActionResult> VerifyCode(string mobile, string code)
    {
        try
        {
            string? ValidCode = await _cache.GetAsync<string>($"otp:{mobile}");
            var dto = new VerifyCodeDto()
            {
                Code = code,
                ValidCode = ValidCode
            };

            // Validation via FluentValidation
            _verifyCodeValidator.ValidateAndThrow(dto);

            bool isValid = string.Equals(dto.Code, dto.ValidCode, StringComparison.Ordinal);
            if (!isValid)
                return BadRequest(new { error = "Invalid code" });

            return Ok(new { message = "Valid code" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "VerifyCode failed");
            throw;
        }
    }
}
