using System.Net.Http.Json;
using AuthenticationService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AuthenticationService.Infrastructure.Otp;

public class OtpHttpClient : IOtpClient
{
    private readonly HttpClient _http;

    public OtpHttpClient(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        var baseUrl = configuration["Otp:BaseUrl"] ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(baseUrl))
            _http.BaseAddress = new Uri(baseUrl);
    }

    public async Task SendCodeAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var payload = new { PhoneNumber = phoneNumber };
        using var resp = await _http.PostAsJsonAsync("api/Otp/Send", payload, cancellationToken);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<bool> VerifyCodeAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
    {
        var payload = new { PhoneNumber = phoneNumber, Code = code };
        using var resp = await _http.PostAsJsonAsync("api/Otp/Verify", payload, cancellationToken);
        resp.EnsureSuccessStatusCode();
        var ok = await resp.Content.ReadFromJsonAsync<bool>(cancellationToken: cancellationToken);
        return ok;
    }
}

