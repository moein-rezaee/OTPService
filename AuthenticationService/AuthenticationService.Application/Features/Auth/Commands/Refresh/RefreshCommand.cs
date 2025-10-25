using AuthenticationService.Application.Features.Auth.Models;
using AuthenticationService.Domain.Entities;
using MediatR;

namespace AuthenticationService.Application.Features.Auth.Commands.Refresh;

public record RefreshCommand(RefreshRequest Request) : IRequest<(User User, string AccessToken, string RefreshToken)>;

