using TaskManager.Application.Common;
using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks;

public static class TaskRules
{
    public static void ValidateDueDateOnCreate(DateTimeOffset? dueDate, DateTimeOffset now)
    {
        if (dueDate is null)
        {
            return;
        }

        if (dueDate.Value < now)
        {
            throw new ValidationException("Due date cannot be in the past when the task is created.");
        }
    }

    public static void ValidateStatusTransition(TaskItem task, TaskItemStatus newStatus)
    {
        if (task.Status == newStatus)
        {
            return;
        }

        var ok =
            (task.Status == TaskItemStatus.ToDo && newStatus == TaskItemStatus.InProgress) ||
            (task.Status == TaskItemStatus.InProgress && newStatus == TaskItemStatus.Done);

        if (!ok)
        {
            throw new ValidationException($"Invalid status transition: {task.Status} -> {newStatus}.");
        }

        if (newStatus == TaskItemStatus.Done && string.IsNullOrWhiteSpace(task.Description))
        {
            throw new ValidationException("A task cannot be marked as done if the description is empty.");
        }
    }
}

