using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.TestHub;

[Table("AnswerOptions")]
public class AnswerOptionEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    [Required]
    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }

    public QuestionEntity? Question { get; set; }
}
