using CustomResponce.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OTPService.Common;
using OTPService.DTOs;
using OTPService.Services;

namespace OTPService.Controllers;

[ApiController]
[Route("[action]")]
public class SmsController(
    IValidator<SendSmsDto> sendCodeValidator,
    ILogger<SmsController> logger) : ControllerBase
{
    private readonly ILogger<SmsController> _logger = logger;
    private readonly IValidator<SendSmsDto> _sendCodeValidator = sendCodeValidator;
    private readonly SmsService _service = new();


    [HttpPost]
    public async Task<IActionResult> Send(SendSmsDto dto)
    {
        var result = new Result();
        try
        {
            //Validation
            var check = _sendCodeValidator.Validate(dto);
            if (!check.IsValid)
            {
                result = CustomErrors.InvalidInputData(check.Errors);
                return StatusCode(result.StatusCode, result);
            }

            result = await _service.SendSms(dto);
            return StatusCode(result.StatusCode, result);
        }
        catch (Exception e)
        {
            _logger.LogInformation(e.Message);
            result = CustomErrors.SendSmsServerError(e);
            return StatusCode(result.StatusCode, result);
        }
    }
}
