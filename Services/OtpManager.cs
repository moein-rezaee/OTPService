using OTPService.DTOs;
using SmsExtension.Core;
using SmsExtension.Abstractions;
using CacheExtension.Abstractions;
using Microsoft.Extensions.Logging;

namespace OTPService.Services
{
    public class OtpManager
    {
        private readonly ISmsProvider _smsProvider;
        private readonly ICacheService _cache;
        private readonly ILogger<OtpManager> _logger;
        private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(2);

        public OtpManager(
            ISmsProvider smsProvider,
            ICacheService cache,
            ILogger<OtpManager> logger)
        {
            _smsProvider = smsProvider;
            _cache = cache;
            _logger = logger;
        }

        public async Task<string> SendCode(SendCodeDto dto)
        {
            var code = new Random().Next(1000, 9999).ToString();
            dto.Message = string.IsNullOrWhiteSpace(dto.Message)
                ? "کد احراز هویت شما جهت ورود به سامانه: " + code
                : dto.Message;
            
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

            await _cache.SetAsync($"otp:{dto.Mobile}", code, OtpTtl);
            return code;
        }

        public async Task<bool> VerifyCode(string mobile, string code)
        {
            var validCode = await _cache.GetAsync<string>($"otp:{mobile}");
            var isValid = string.Equals(code, validCode, StringComparison.Ordinal);
            return isValid;
        }
    }
}
