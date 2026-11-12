namespace Application.Models.Tests;

/// <summary>Тест для проходження: питання з варіантами відповідей БЕЗ ознаки правильності.</summary>
public record TestPassDto(
    int Id,
    string Title,
    int? TimeLimitMinutes,
    IReadOnlyList<QuestionPassDto> Questions);

/// <param name="IsMultipleChoice">true — правильних відповідей кілька (UI показує чекбокси).</param>
public record QuestionPassDto(
    int Id,
    string Text,
    int OrderIndex,
    int Points,
    bool IsMultipleChoice,
    IReadOnlyList<AnswerOptionPassDto> Options);

public record AnswerOptionPassDto(int Id, string Text);
