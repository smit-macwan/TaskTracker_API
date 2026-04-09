using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Tasks;
using TaskManager.Application.Tasks.Dtos;

namespace TaskManager.WebApi.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public sealed class TasksController(TaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> ListMine(CancellationToken ct)
    {
        var tasks = await taskService.ListMyTasksAsync(ct);
        return Ok(tasks);
    }

    [HttpGet("all")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> ListAll(CancellationToken ct)
    {
        var tasks = await taskService.ListAllTasksAsync(ct);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(CreateTaskRequest request, CancellationToken ct)
    {
        var created = await taskService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(ListMine), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskDto>> Update(Guid id, UpdateTaskRequest request, CancellationToken ct)
    {
        var updated = await taskService.UpdateAsync(id, request, ct);
        return Ok(updated);
    }

    [HttpPost("{id:guid}/status")]
    public async Task<ActionResult<TaskDto>> ChangeStatus(Guid id, ChangeStatusRequest request, CancellationToken ct)
    {
        var updated = await taskService.ChangeStatusAsync(id, request.NewStatus, ct);
        return Ok(updated);
    }
}

