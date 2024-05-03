namespace OTPService.DTOs
{
    public class VerifyCodeDto
    {
        public required string Code { get; set; }
        public string? ValidCode { get; set; }

    }
}