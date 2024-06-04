namespace OTPService.DTOs
{
    public class SendCodeDto()
    {
        public Guid OrganizationId { get; set; }
        public required string Mobile { get; set; }
    }
}