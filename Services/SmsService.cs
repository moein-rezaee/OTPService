using OTPService.DTOs;
using SmsExtension.Core;
using Microsoft.Extensions.Logging;

namespace OTPService.Services
{
    public class SmsService
    {
        private readonly ISmsProvider _smsProvider;
        private readonly ILogger<SmsService> _logger;

        public SmsService(ISmsProvider smsProvider, ILogger<SmsService> logger)
        {
            _smsProvider = smsProvider;
            _logger = logger;
        }


        public async Task<string> SendCode(SendCodeDto dto)
        {
            var code = new Random().Next(1000, 9999).ToString();
            dto.Message = string.IsNullOrWhiteSpace(dto.Message)
                ? "کد احراز هویت شما جهت ورود به سامانه: " + code
                : dto.Message;

            if (string.IsNullOrWhiteSpace(dto.Mobile) || string.IsNullOrWhiteSpace(dto.Message))
            {
                _logger.LogWarning("Invalid SMS input. Mobile or Message is empty");
                throw new ArgumentException("Mobile and Message are required");
            }

            var sendResult = await _smsProvider.SendAsync(new SmsMessage
            {
                Mobile = dto.Mobile,
                Text = dto.Message
            });

            if (!sendResult.Success)
            {
                _logger.LogError("SMS send failed for {Mobile}: {Error}", dto.Mobile, sendResult.ErrorMessage);
                throw new InvalidOperationException("SMS send failed");
            }

            return code;
        }
    }
}
