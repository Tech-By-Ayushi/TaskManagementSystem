using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Task.Shared;
namespace Task.Api.Models;

public class TaskItem
{
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public Shared.TaskStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public string? CreatedById { get; set; }
    public IdentityUser? CreatedBy { get; set; }
}