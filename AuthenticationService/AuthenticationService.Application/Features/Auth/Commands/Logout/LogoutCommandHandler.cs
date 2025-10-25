using AuthenticationService.Domain.Entities;
using AuthenticationService.Domain.Interfaces;
using MediatR;

namespace AuthenticationService.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler(IUnitOfWork uow) : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IUnitOfWork _uow = uow;
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<RefreshToken>();
        var token = await repo.FirstOrDefaultAsync(x => x.Token == request.Request.RefreshToken && x.RevokedAt == null, cancellationToken);
        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
}

