using AuthenticationService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationService.Infrastructure.Data;

public class UnitOfWork(IServiceProvider services, DbContext context) : IUnitOfWork
{
    private readonly IServiceProvider _services = services;
    private readonly DbContext _context = context;

    public IRepository<T> Repository<T>() where T : class
        => ActivatorUtilities.CreateInstance<EfRepository<T>>(_services, _context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}

