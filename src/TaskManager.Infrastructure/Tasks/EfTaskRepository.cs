using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Tasks;
using TaskManager.Domain.Tasks;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Tasks;

public sealed class EfTaskRepository(AppDbContext db) : ITaskRepository
{
    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Tasks.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<TaskItem>> ListForUserAsync(string userId, CancellationToken ct)
    {
        var items = await db.Tasks
            .Where(x => x.OwnerUserId == userId)
            .ToListAsync(ct);

        return items
            .OrderByDescending(x => x.UpdatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<TaskItem>> ListAllAsync(CancellationToken ct)
    {
        var items = await db.Tasks.ToListAsync(ct);

        return items
            .OrderByDescending(x => x.UpdatedAt)
            .ToList();
    }

    public void Add(TaskItem task) => db.Tasks.Add(task);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}

