using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTodoList.Data;

[Table("tblTasks")]
public class TaskEntity
{
    [Key]
    public int Id { get; set; }
    [Required, StringLength(255)]
    public string Name { get; set; } = string.Empty;
    [StringLength(4000)]
    public string? Description { get; set; }
    public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; }
    public DateTime DeadLine { get; set; }
    public DateTime? DateCompleted { get; set; }
}
