namespace OTPService.DTOs
{
    public class SendSmsDto
    {
        public required string Message { get; set; }
        public required string Receivers { get; set; }

    }
}