using AuthenticationService.Application.Features.Auth.Models;
using MediatR;

namespace AuthenticationService.Application.Features.Auth.Commands.Send;

public record SendCommand(SendCodeRequest Request) : IRequest<Unit>;

