using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OTPService.DTOs;
using OTPService.Services;

namespace OTPService.Controllers;

[ApiController]
[Route("[action]")]
public class SmsController : ControllerBase
{
    private readonly ILogger<SmsController> _logger;
    private readonly IValidator<SendCodeDto> _sendCodeValidator;
    private readonly IValidator<VerifyCodeDto> _verifyCodeValidator;
    private readonly SmsService _service;
    private readonly string SESSION_KEY = "Code";



    public SmsController(
        IValidator<SendCodeDto> sendCodeValidator,
        IValidator<VerifyCodeDto> verifyCodeValidator,
        SmsService service,
        ILogger<SmsController> logger)
    {
        _logger = logger;
        _sendCodeValidator = sendCodeValidator;
        _verifyCodeValidator = verifyCodeValidator;
        _service = service;

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
            //Validation
            var check = _sendCodeValidator.Validate(dto);
            if (!check.IsValid)
                throw new ValidationException(check.Errors);

            var code = await _service.SendCode(dto);
            if (!string.IsNullOrEmpty(code))
            {
                HttpContext.Session.SetString(SESSION_KEY, code);
            }

            return Ok(new { message = "Code sent" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SendCode failed for {Mobile}", mobile);
            throw;
        }
    }

    [HttpGet("{code}")]
    public IActionResult VerifyCode(string code)
    {
        try
        {
            string? ValidCode = HttpContext.Session.GetString(SESSION_KEY);
            var dto = new VerifyCodeDto()
            {
                Code = code,
                ValidCode = ValidCode
            };

            var check = _verifyCodeValidator.Validate(dto);
            if (!check.IsValid)
                throw new ValidationException(check.Errors);

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
