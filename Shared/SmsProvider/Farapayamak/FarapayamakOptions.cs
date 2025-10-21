using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using Shared.SmsProvider.Core;
using Shared.SmsProvider.Abstractions;

namespace Shared.SmsProvider.Farapayamak
{
    public class FarapayamakOptions
    {
        [Required(ErrorMessage = "Farapayamak API key is required")]
        public string ApiKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Farapayamak username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Farapayamak password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Farapayamak sender number is required")]
        public string Sender { get; set; } = string.Empty;
    }
}