using AuthenticationService.Application.Features.Auth.Models;
using MediatR;

namespace AuthenticationService.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(LogoutRequest Request) : IRequest<Unit>;

