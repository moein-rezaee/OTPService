using AuthenticationService.Application.Features.Auth.Services;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Domain.Interfaces;
using MediatR;

namespace AuthenticationService.Application.Features.Auth.Commands.Refresh;

public class RefreshCommandHandler(IUnitOfWork uow, IJwtService jwt) : IRequestHandler<RefreshCommand, (User User, string AccessToken, string RefreshToken)>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IJwtService _jwt = jwt;

    public async Task<(User User, string AccessToken, string RefreshToken)> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var rtRepo = _uow.Repository<RefreshToken>();
        var token = await rtRepo.FirstOrDefaultAsync(x => x.Token == request.Request.RefreshToken && x.RevokedAt == null, cancellationToken);
        if (token is null || token.ExpiresAt <= DateTime.UtcNow)
            throw new InvalidOperationException("Invalid refresh token");

        var user = await _uow.Repository<User>().GetByIdAsync(token.UserId, cancellationToken)
                   ?? throw new InvalidOperationException("User not found");

        // rotate token
        token.RevokedAt = DateTime.UtcNow;
        var newRefresh = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = _jwt.GenerateRefreshToken(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        await rtRepo.AddAsync(newRefresh, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var access = _jwt.GenerateAccessToken(user, out _);
        return (user, access, newRefresh.Token);
    }
}

