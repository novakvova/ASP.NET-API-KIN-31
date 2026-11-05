using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.TestHub;

[Table("Tests")]
public class TestEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Ліміт часу в хвилинах (необов'язково)
    [Range(1, 600)]
    public int? TimeLimitMinutes { get; set; }

    public ICollection<QuestionEntity> Questions { get; set; } = new List<QuestionEntity>();
}
