using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task.Api.Models;

namespace Task.Api.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<TaskItem> Tasks { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}