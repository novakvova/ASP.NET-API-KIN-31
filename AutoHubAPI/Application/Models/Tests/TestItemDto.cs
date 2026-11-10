namespace Application.Models.Tests;

/// <summary>Елемент списку тестів.</summary>
public record TestItemDto(
    int Id,
    string Title,
    string? Description,
    int? TimeLimitMinutes,
    DateTime CreatedAt,
    int QuestionsCount);
