using System.ComponentModel.DataAnnotations;

namespace Shared.SmsProvider.Kavenegar
{
    public class KavenegarOptions
    {
        [Required(ErrorMessage = "Kavenegar API key is required")]
        public string ApiKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kavenegar sender number is required")]
        public string Sender { get; set; } = string.Empty;
    }
}