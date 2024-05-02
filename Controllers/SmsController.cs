using CustomResponce.Models;
using Fetch;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OTPService.Common;
using OTPService.DTOs;

namespace OTPService.Controllers;

[ApiController]
[Route("[action]")]
public class SmsController : ControllerBase
{
    private readonly ILogger<SmsController> _logger;
    private readonly FetchHttpRequest _fetch;
    private readonly IValidator<SendCodeDto> _sendCodeValidator;
    private readonly IValidator<VerifyCodeDto> _verifyCodeValidator;
    public SmsController(
        IValidator<SendCodeDto> sendCodeValidator,
        IValidator<VerifyCodeDto> verifyCodeValidator,
        IHttpClientFactory httpClientFactory,
        ILogger<SmsController> logger)
    {
        _logger = logger;
        _sendCodeValidator = sendCodeValidator;
        _verifyCodeValidator = verifyCodeValidator;

        FetchClientOptions fetchClientOptions = new()
        {
            BaseUrl = "https://api.kavenegar.com/v1/"
        };
        _fetch = new(httpClientFactory, fetchClientOptions);
    }

    [HttpGet("{mobile}")]
    public async Task<IActionResult> SendCode(string mobile)
    {
        var result = new Result();
        var code = new Random().Next(1000, 9999).ToString();
        var dto = new SendCodeDto()
        {
            Mobile = mobile,
            Message = "کد احراز هویت شما جهت ورود به سامانه: " + code,
            SenderNumber = ""
        };

        try
        {
            var check = _sendCodeValidator.Validate(dto);
            if (!check.IsValid)
            {
                result = CustomErrors.InvalidMobileNumber();
                return StatusCode(result.StatusCode, result);
            }

            // Send Code
            string API_KEY = "51566F5254736B6161375775564279316957454E4F5436527A6E70536756454E";
            FetchRequestOptions options = new()
            {
                Url = $@"{API_KEY}/sms/send.json",
                Params = $@"?receptor={dto.Mobile}&sender={dto.SenderNumber}&message={dto.Message}"
            };
            var responce = await _fetch.Get(options);

            if (!responce.Status)
            {
                result = CustomErrors.SendCodeFailed();
                return StatusCode(result.StatusCode, result);
            }

            result = CustomResults.CodeSent();
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
    public async Task<IActionResult> VerifyCode(string code)
    {
        var result = new Result();
        var dto = new VerifyCodeDto()
        {
            Code = code
        };

        try
        {
            var check = _verifyCodeValidator.Validate(dto);
            if (!check.IsValid)
            {
                result = CustomErrors.InvalidMobileNumber();
                return StatusCode(result.StatusCode, result);
            }

            // Verify Code

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
