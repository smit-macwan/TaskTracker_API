namespace TaskManager.Application.Tasks.Dtos;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    DateTimeOffset? DueDate
);

