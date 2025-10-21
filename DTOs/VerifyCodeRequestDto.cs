namespace OTPService.DTOs;

public class VerifyCodeRequestDto
{
    public string Mobile { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

