namespace Application.Models.Seed;

/// <summary>Тест із файлу SeedData/tests.json.</summary>
public class TestSeedModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? TimeLimitMinutes { get; set; }
    public List<QuestionSeedModel> Questions { get; set; } = new List<QuestionSeedModel>();
}

public class QuestionSeedModel
{
    public string Text { get; set; } = string.Empty;
    public int Points { get; set; } = 1;
    public List<AnswerSeedModel> Answers { get; set; } = new List<AnswerSeedModel>();
}

public class AnswerSeedModel
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
