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
    private readonly IValidator<VerifyCodeRequestDto> _verifyCodeRequestValidator;
    private readonly OtpManager _service;



    public SmsController(
        IValidator<SendCodeDto> sendCodeValidator,
        IValidator<VerifyCodeRequestDto> verifyCodeRequestValidator,
        OtpManager service,
        ILogger<SmsController> logger)
    {
        _logger = logger;
        _sendCodeValidator = sendCodeValidator;
        _verifyCodeRequestValidator = verifyCodeRequestValidator;
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
            // Validation via FluentValidation
            _sendCodeValidator.ValidateAndThrow(dto);

            await _service.SendCode(dto);

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
            // Validate inputs using a single DTO
            _verifyCodeRequestValidator.ValidateAndThrow(new VerifyCodeRequestDto { Mobile = mobile, Code = code });

            bool isValid = await _service.VerifyCode(mobile, code);
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
