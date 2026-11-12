namespace Application.Models.Tests;

/// <summary>Результат проходження тесту.</summary>
public record TestResultDto(
    int TestId,
    string Title,
    int Score,
    int MaxScore,
    double Percent,
    int CorrectCount,
    int QuestionsCount,
    IReadOnlyList<QuestionResultDto> Questions);

public record QuestionResultDto(
    int QuestionId,
    string Text,
    int Points,
    int EarnedPoints,
    bool IsCorrect,
    IReadOnlyList<int> SelectedOptionIds,
    IReadOnlyList<int> CorrectOptionIds);
