namespace TaskManager.Domain.Tasks;

public sealed class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string OwnerUserId { get; set; } = default!;

    public string Title { get; set; } = default!;
    public string? Description { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.ToDo;

    public DateTimeOffset? DueDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

