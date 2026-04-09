using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.Tasks.Dtos;
using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks;

public sealed class TaskService(ITaskRepository repo, ICurrentUser currentUser)
{
    public async Task<IReadOnlyList<TaskDto>> ListMyTasksAsync(CancellationToken ct)
    {
        var tasks = await repo.ListForUserAsync(currentUser.UserId, ct);
        return tasks.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<TaskDto>> ListAllTasksAsync(CancellationToken ct)
    {
        if (!currentUser.IsAdmin)
        {
            throw new ValidationException("Only admin users can view all tasks.");
        }

        var tasks = await repo.ListAllAsync(ct);
        return tasks.Select(ToDto).ToList();
    }

    public async Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Title is required.");
        }

        var now = DateTimeOffset.UtcNow;
        TaskRules.ValidateDueDateOnCreate(request.DueDate, now);

        var task = new TaskItem
        {
            OwnerUserId = currentUser.UserId,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            DueDate = request.DueDate,
            Status = TaskItemStatus.ToDo,
            CreatedAt = now,
            UpdatedAt = now
        };

        repo.Add(task);
        await repo.SaveChangesAsync(ct);

        return ToDto(task);
    }

    public async Task<TaskDto> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Title is required.");
        }

        var task = await repo.GetByIdAsync(id, ct) ?? throw new NotFoundException("Task not found.");
        EnsureAccess(task);

        task.Title = request.Title.Trim();
        task.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTimeOffset.UtcNow;

        await repo.SaveChangesAsync(ct);
        return ToDto(task);
    }

    public async Task<TaskDto> ChangeStatusAsync(Guid id, TaskItemStatus newStatus, CancellationToken ct)
    {
        var task = await repo.GetByIdAsync(id, ct) ?? throw new NotFoundException("Task not found.");
        EnsureAccess(task);

        TaskRules.ValidateStatusTransition(task, newStatus);
        task.Status = newStatus;
        task.UpdatedAt = DateTimeOffset.UtcNow;

        await repo.SaveChangesAsync(ct);
        return ToDto(task);
    }

    private void EnsureAccess(TaskItem task)
    {
        if (currentUser.IsAdmin)
        {
            return;
        }

        if (!string.Equals(task.OwnerUserId, currentUser.UserId, StringComparison.Ordinal))
        {
            throw new ValidationException("You can only access your own tasks.");
        }
    }

    private static TaskDto ToDto(TaskItem t) =>
        new(
            t.Id,
            t.Title,
            t.Description,
            t.Status,
            t.DueDate,
            t.CreatedAt,
            t.UpdatedAt
        );
}

