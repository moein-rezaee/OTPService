using CustomResponce.Models;
using Fetch;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OTPService.Common;
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
        IHttpClientFactory httpClientFactory,
        ILogger<SmsController> logger)
    {
        _logger = logger;
        _sendCodeValidator = sendCodeValidator;
        _verifyCodeValidator = verifyCodeValidator;
        _service = new SmsService(httpClientFactory);

    }

    [HttpGet("{mobile}")]
    public async Task<IActionResult> SendCode(string mobile)
    {
        var result = new Result();
        var dto = new SendCodeDto()
        {
            Mobile = mobile,
        };

        try
        {
            //Validation
            var check = _sendCodeValidator.Validate(dto);
            if (!check.IsValid)
            {
                result = CustomErrors.InvalidMobileNumber();
                return StatusCode(result.StatusCode, result);
            }

            result = await _service.SendCode(dto);
            string? code = result.Data?.ToString() ?? null;
            if (result.Status && !string.IsNullOrEmpty(code))
            {
                HttpContext.Session.SetString(SESSION_KEY, code);
                // FIXME: Uncomment
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

    [HttpGet("{code}")]
    public IActionResult VerifyCode(string code)
    {
        var result = new Result();

        try
        {
            string? ValidCode = HttpContext.Session.GetString(SESSION_KEY);
            var dto = new VerifyCodeDto()
            {
                Code = code,
                ValidCode = ValidCode
            };

            var check = _verifyCodeValidator.Validate(dto);
            if (check.IsValid)
                result = CustomResults.ValidCode();
            else
                result = CustomErrors.InvalidCode();

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
