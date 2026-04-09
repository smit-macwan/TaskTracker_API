using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks.Dtos;

public sealed record ChangeStatusRequest(TaskItemStatus NewStatus);

