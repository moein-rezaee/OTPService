namespace AuthenticationService.Application.Features.Auth.Models;

public record SendCodeRequest(string PhoneNumber);
public record VerifyCodeRequest(string PhoneNumber, string Code);
public record TokenResponse(string AccessToken, string RefreshToken);
public record RefreshRequest(string RefreshToken);
public record LogoutRequest(string RefreshToken);

