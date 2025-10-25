using AuthenticationService.Application.Features.Auth.Models;
using AuthenticationService.Domain.Entities;
using MediatR;

namespace AuthenticationService.Application.Features.Auth.Commands.Verify;

public record VerifyCommand(VerifyCodeRequest Request) : IRequest<(User User, string AccessToken, string RefreshToken)>;

