using AuthenticationService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuthenticationService.Infrastructure.Data;

public class EfRepository<T>(DbContext context) : IRepository<T> where T : class
{
    private readonly DbContext _context = context;
    private readonly DbSet<T> _set = context.Set<T>();

    public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        => await _set.FindAsync([id], cancellationToken);

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _set.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _set.AddAsync(entity, cancellationToken);
    }

    public IQueryable<T> Query() => _set.AsQueryable();
}

