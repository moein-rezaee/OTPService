namespace OTPService.Domain.Interfaces;

public interface IOtpService
{
    Task<string> GenerateCodeAsync(string phoneNumber);
    Task<bool> ValidateCodeAsync(string phoneNumber, string code);
    Task SendCodeAsync(string phoneNumber);
}
