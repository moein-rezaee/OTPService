using AuthenticationService.Application.Features.Auth.Services;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Domain.Interfaces;
using MediatR;

namespace AuthenticationService.Application.Features.Auth.Commands.Verify;

public class VerifyCommandHandler(
    IOtpClient otpClient,
    IUnitOfWork uow,
    IJwtService jwt) : IRequestHandler<VerifyCommand, (User User, string AccessToken, string RefreshToken)>
{
    private readonly IOtpClient _otpClient = otpClient;
    private readonly IUnitOfWork _uow = uow;
    private readonly IJwtService _jwt = jwt;

    public async Task<(User User, string AccessToken, string RefreshToken)> Handle(VerifyCommand request, CancellationToken cancellationToken)
    {
        var ok = await _otpClient.VerifyCodeAsync(request.Request.PhoneNumber, request.Request.Code, cancellationToken);
        if (!ok) throw new InvalidOperationException("Invalid code");

        var userRepo = _uow.Repository<User>();
        var user = await userRepo.FirstOrDefaultAsync(x => x.PhoneNumber == request.Request.PhoneNumber, cancellationToken);
        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                PhoneNumber = request.Request.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };
            await userRepo.AddAsync(user, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        var access = _jwt.GenerateAccessToken(user, out _);
        var refresh = _jwt.GenerateRefreshToken();

        var rtRepo = _uow.Repository<RefreshToken>();
        await rtRepo.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refresh,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        }, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return (user, access, refresh);
    }
}

