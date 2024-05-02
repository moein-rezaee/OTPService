namespace OTPService.DTOs
{
    public class SendCodeDto
    {
        public required string Mobile { get; set; }
        public required string Message { get; set; }
        public required string SenderNumber { get; set; }
    }
}