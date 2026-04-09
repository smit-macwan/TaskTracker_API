using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<TaskItem>> ListForUserAsync(string userId, CancellationToken ct);
    Task<IReadOnlyList<TaskItem>> ListAllAsync(CancellationToken ct);
    void Add(TaskItem task);
    Task SaveChangesAsync(CancellationToken ct);
}

