using Demo.Api.Notifications.Domain;
using Microsoft.EntityFrameworkCore;

namespace Demo.Api.Notifications.Infrastructure.Database;

public interface INotificationsRepository
{
    Task<List<Notification>> ListAsync(CancellationToken cancellationToken = default);
    Task<Notification?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}

public sealed class NotificationsRepository(DatabaseContext Context) : INotificationsRepository
{
    private readonly DatabaseContext _context = Context;

    public Task<List<Notification>> ListAsync(CancellationToken cancellationToken = default)
        => _context.Notifications.ToListAsync(cancellationToken);

    public Task<Notification?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Notifications.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _context.Update(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
