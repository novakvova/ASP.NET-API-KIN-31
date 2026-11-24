using Microsoft.EntityFrameworkCore;

namespace WebTodoList.Data;

public class AppTodoDbContext : DbContext
{
    public AppTodoDbContext(DbContextOptions<AppTodoDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<TaskEntity> Tasks { get; set; }

}
