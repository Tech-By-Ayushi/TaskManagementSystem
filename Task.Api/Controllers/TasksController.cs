using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task.Api.Data;
using Task.Api.Models;
using Task.Shared;

namespace Task.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TasksController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            IQueryable<TaskItem> query = _context.Tasks.Include(t => t.CreatedBy);

            if (!isAdmin)
            {
                query = query.Where(t => t.CreatedById == userId);
            }

            var tasks = await query
                .Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    DueDate = t.DueDate,
                    CreatedByEmail = t.CreatedBy!.Email
                })
                .ToListAsync();

            return Ok(tasks);
        }

        // GET: api/tasks/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TaskItemDto>> GetTaskById(Guid id)
        {
            var taskItem = await _context.Tasks
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (taskItem == null)
            {
                return NotFound();
            }

            var taskDto = new TaskItemDto
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                Status = taskItem.Status,
                CreatedAt = taskItem.CreatedAt,
                DueDate = taskItem.DueDate,
                AssignedToUserId = taskItem.CreatedById,
                CreatedByEmail = taskItem.CreatedBy?.Email
            };

            return Ok(taskDto);
        }

        // POST: api/tasks
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TaskItemDto>> CreateTask(TaskItemDto taskDto)
        {
            var userToAssign = await _userManager.FindByIdAsync(taskDto.AssignedToUserId!);
            if (userToAssign == null)
            {
                return BadRequest("The user you are assigning this task to does not exist.");
            }

            var taskItem = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status,
                CreatedAt = DateTime.UtcNow,
                DueDate = taskDto.DueDate,
                CreatedById = taskDto.AssignedToUserId
            };

            _context.Tasks.Add(taskItem);
            await _context.SaveChangesAsync();

            taskDto.Id = taskItem.Id;
            taskDto.CreatedAt = taskItem.CreatedAt;
            taskDto.CreatedByEmail = userToAssign.Email;

            return CreatedAtAction(nameof(GetTasks), new { id = taskItem.Id }, taskDto);
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTask(Guid id, TaskItemDto taskDto)
        {
            if (id != taskDto.Id)
            {
                return BadRequest();
            }

            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            taskItem.Title = taskDto.Title;
            taskItem.Description = taskDto.Description;
            taskItem.DueDate = taskDto.DueDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/tasks/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(Guid id, [FromBody] Shared.TaskStatus status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var taskItem = await _context.Tasks.FindAsync(id);

            if (taskItem == null)
            {
                return NotFound();
            }

            if (taskItem.CreatedById != userId && !isAdmin)
            {
                return Forbid();
            }

            taskItem.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}