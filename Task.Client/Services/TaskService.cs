using System.Net.Http.Json;
using Task.Shared;

namespace Task.Client.Services;

public class TaskService
{
    private readonly HttpClient _httpClient;

    public TaskService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TaskItemDto>?> GetTasksAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TaskItemDto>>("api/tasks");
    }

    public async ValueTask AddTaskAsync(TaskItemDto task)
    {
        await _httpClient.PostAsJsonAsync("api/tasks", task);
    }

    public async ValueTask UpdateTaskStatusAsync(Guid id, Shared.TaskStatus newStatus)
    {
        await _httpClient.PutAsJsonAsync($"api/tasks/{id}/status", newStatus);
    }

    public async ValueTask DeleteTaskAsync(Guid id)
    {
        await _httpClient.DeleteAsync($"api/tasks/{id}");
    }
    

    public async Task<TaskItemDto?> GetTaskByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<TaskItemDto>($"api/tasks/{id}");
    }

    public async ValueTask UpdateTaskAsync(Guid id, TaskItemDto task)
    {
        await _httpClient.PutAsJsonAsync($"api/tasks/{id}", task);
    }
}