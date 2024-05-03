namespace OTPService.DTOs
{
    public class SendCodeDto
    {

        public required string Mobile { get; set; }
        public string? Message { get; set; }
        public string? SenderNumber { get; set; }

    }
}