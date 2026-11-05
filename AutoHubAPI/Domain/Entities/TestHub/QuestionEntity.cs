using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.TestHub;

[Table("Questions")]
public class QuestionEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    // Порядок питання в тесті
    public int OrderIndex { get; set; }

    [Range(1, 100)]
    public int Points { get; set; } = 1;

    [Required]
    [ForeignKey(nameof(Test))]
    public int TestId { get; set; }

    public TestEntity? Test { get; set; }

    public ICollection<AnswerOptionEntity> AnswerOptions { get; set; } = new List<AnswerOptionEntity>();
}