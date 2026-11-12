using System.ComponentModel.DataAnnotations;

namespace Application.Models.Tests;

/// <summary>Відповіді користувача на тест.</summary>
public class TestSubmitRequest
{
    [Required]
    public List<QuestionAnswerRequest> Answers { get; set; } = new List<QuestionAnswerRequest>();
}

public class QuestionAnswerRequest
{
    [Range(1, int.MaxValue)]
    public int QuestionId { get; set; }

    public List<int> SelectedOptionIds { get; set; } = new List<int>();
}
