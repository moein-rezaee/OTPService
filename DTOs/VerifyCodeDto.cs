namespace OTPService.DTOs
{
    public class VerifyCodeDto
    {
        public Guid OrganizationId { get; set; }
        public required string Mobile { get; set; }
        public required string Code { get; set; }
    }
}