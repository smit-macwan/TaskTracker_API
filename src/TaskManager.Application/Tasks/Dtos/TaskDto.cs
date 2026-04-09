using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks.Dtos;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

