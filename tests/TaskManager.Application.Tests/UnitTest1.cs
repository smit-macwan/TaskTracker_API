using System.Security.Claims;
using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.Tasks;
using TaskManager.Application.Tasks.Dtos;
using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tests;

public sealed class TaskRulesTests
{
    [Fact]
    public void ValidateDueDateOnCreate_Throws_WhenDueDateIsInPast()
    {
        var now = new DateTimeOffset(2026, 4, 9, 10, 0, 0, TimeSpan.Zero);
        var dueDate = now.AddMinutes(-1);

        var ex = Assert.Throws<ValidationException>(() => TaskRules.ValidateDueDateOnCreate(dueDate, now));
        Assert.Equal("Due date cannot be in the past when the task is created.", ex.Message);
    }

    [Fact]
    public void ValidateStatusTransition_Throws_WhenTransitionIsInvalid()
    {
        var task = new TaskItem { Status = TaskItemStatus.ToDo, Description = "Has details" };

        var ex = Assert.Throws<ValidationException>(() => TaskRules.ValidateStatusTransition(task, TaskItemStatus.Done));
        Assert.Equal("Invalid status transition: ToDo -> Done.", ex.Message);
    }

    [Fact]
    public void ValidateStatusTransition_Throws_WhenMarkingDoneWithoutDescription()
    {
        var task = new TaskItem { Status = TaskItemStatus.InProgress, Description = null };

        var ex = Assert.Throws<ValidationException>(() => TaskRules.ValidateStatusTransition(task, TaskItemStatus.Done));
        Assert.Equal("A task cannot be marked as done if the description is empty.", ex.Message);
    }
}

public sealed class TaskServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsTaskForCurrentUser()
    {
        var repo = new InMemoryTaskRepository();
        var currentUser = new FakeCurrentUser("user-1", isAdmin: false);
        var sut = new TaskService(repo, currentUser);

        var result = await sut.CreateAsync(
            new CreateTaskRequest("Title", "Description", DateTimeOffset.UtcNow.AddDays(1)),
            CancellationToken.None);

        Assert.Equal("Title", result.Title);
        Assert.Equal(TaskItemStatus.ToDo, result.Status);
        Assert.Single(repo.Items);
        Assert.Equal("user-1", repo.Items[0].OwnerUserId);
        Assert.Equal(1, repo.SaveChangesCallCount);
    }

    [Fact]
    public async Task ListAllTasksAsync_ThrowsForNonAdmin()
    {
        var repo = new InMemoryTaskRepository();
        var currentUser = new FakeCurrentUser("user-1", isAdmin: false);
        var sut = new TaskService(repo, currentUser);

        var ex = await Assert.ThrowsAsync<ValidationException>(() => sut.ListAllTasksAsync(CancellationToken.None));
        Assert.Equal("Only admin users can view all tasks.", ex.Message);
    }

    [Fact]
    public async Task ChangeStatusAsync_Throws_WhenUserAccessesTaskOwnedByAnotherUser()
    {
        var repo = new InMemoryTaskRepository();
        var otherUserTask = new TaskItem
        {
            OwnerUserId = "user-2",
            Title = "Other task",
            Description = "Desc",
            Status = TaskItemStatus.ToDo,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        repo.Items.Add(otherUserTask);

        var currentUser = new FakeCurrentUser("user-1", isAdmin: false);
        var sut = new TaskService(repo, currentUser);

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            sut.ChangeStatusAsync(otherUserTask.Id, TaskItemStatus.InProgress, CancellationToken.None));
        Assert.Equal("You can only access your own tasks.", ex.Message);
    }
}

file sealed class InMemoryTaskRepository : ITaskRepository
{
    public List<TaskItem> Items { get; } = [];
    public int SaveChangesCallCount { get; private set; }

    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

    public Task<IReadOnlyList<TaskItem>> ListForUserAsync(string userId, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<TaskItem>>(
            Items.Where(x => x.OwnerUserId == userId).ToList());

    public Task<IReadOnlyList<TaskItem>> ListAllAsync(CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<TaskItem>>(Items.ToList());

    public void Add(TaskItem task) => Items.Add(task);

    public Task SaveChangesAsync(CancellationToken ct)
    {
        SaveChangesCallCount++;
        return Task.CompletedTask;
    }
}

file sealed class FakeCurrentUser(string userId, bool isAdmin) : ICurrentUser
{
    public string UserId { get; } = userId;
    public bool IsAdmin { get; } = isAdmin;
    public ClaimsPrincipal Principal { get; } = new(new ClaimsIdentity());
}
