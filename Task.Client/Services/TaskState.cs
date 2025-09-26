using Task.Shared;

namespace Task.Client.Services;

public class TaskState
{
    private readonly TaskService _taskService;
    private List<TaskItemDto> _tasks = new();

    // The event that components will subscribe to
    public event Action? OnChange;

    // Public property to expose the state (read-only to prevent direct modification)
    public IReadOnlyList<TaskItemDto> Tasks => _tasks.AsReadOnly();

    public TaskState(TaskService taskService)
    {
        _taskService = taskService;
    }

    // Fetches the initial list of tasks
    public async ValueTask InitializeTasksAsync()
    {
        var tasks = await _taskService.GetTasksAsync();
        _tasks = tasks ?? new List<TaskItemDto>();
        NotifyStateChanged();
    }

    // Method to delete a task, which then notifies components
    public async ValueTask DeleteTaskAsync(Guid taskId)
    {
        await _taskService.DeleteTaskAsync(taskId);
        var taskToRemove = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (taskToRemove != null)
        {
            _tasks.Remove(taskToRemove);
            NotifyStateChanged();
        }
    }

    // Method to update status, which then notifies components
    public async ValueTask UpdateTaskStatusAsync(Guid taskId, Shared.TaskStatus newStatus)
    {
        await _taskService.UpdateTaskStatusAsync(taskId, newStatus);
        var taskToUpdate = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (taskToUpdate != null)
        {
            taskToUpdate.Status = newStatus;
            NotifyStateChanged();
        }
    }

    // The method that triggers the OnChange event
    private void NotifyStateChanged() => OnChange?.Invoke();
}