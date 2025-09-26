using System.ComponentModel.DataAnnotations;

namespace Task.Shared;

public class TaskItemDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public TaskStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public string? CreatedByEmail { get; set; }

    // This property will be used when creating a task
    [Required(ErrorMessage = "Please assign the task to a user.")]
    public string? AssignedToUserId { get; set; }
}