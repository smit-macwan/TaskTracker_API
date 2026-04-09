namespace TaskManager.Application.Tasks.Dtos;

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    DateTimeOffset? DueDate
);

