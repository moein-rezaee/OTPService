using System;

namespace SmsExtension.Provider.Farapayamak;

public class FarapayamakOptions
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
}
