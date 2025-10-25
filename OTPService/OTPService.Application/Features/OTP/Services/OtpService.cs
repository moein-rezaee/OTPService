using System;
using System.Threading.Tasks;
using CacheExtension.Abstractions;
using OTPService.Domain.Interfaces;

namespace OTPService.Application.Features.OTP.Services;

public class OtpService : IOtpService
{
    private readonly ICacheService _cacheService;
    private readonly INotificationClient _notificationClient;
    private const int CODE_LENGTH = 5;
    private const int EXPIRY_MINUTES = 2;

    public OtpService(ICacheService cacheService, INotificationClient notificationClient)
    {
        _cacheService = cacheService;
        _notificationClient = notificationClient;
    }

    public async Task<string> GenerateCodeAsync(string phoneNumber)
    {
        var code = GenerateRandomCode();
        await _cacheService.SetAsync($"otp:{phoneNumber}", code, TimeSpan.FromMinutes(EXPIRY_MINUTES));
        return code;
    }

    public async Task<bool> ValidateCodeAsync(string phoneNumber, string code)
    {
        var storedCode = await _cacheService.GetAsync<string>($"otp:{phoneNumber}");
        return storedCode == code;
    }

    public async Task SendCodeAsync(string phoneNumber)
    {
        var code = await GenerateCodeAsync(phoneNumber);
        await _notificationClient.SendSmsAsync(phoneNumber, code);
    }

    private string GenerateRandomCode()
    {
        var random = new Random();
        return random.Next((int)Math.Pow(10, CODE_LENGTH - 1), (int)Math.Pow(10, CODE_LENGTH)).ToString();
    }
}
