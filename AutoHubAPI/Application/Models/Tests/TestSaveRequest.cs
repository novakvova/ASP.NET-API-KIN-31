using System.ComponentModel.DataAnnotations;

namespace Application.Models.Tests;

/// <summary>Дані для створення або редагування тесту.</summary>
public class TestSaveRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Range(1, 600)]
    public int? TimeLimitMinutes { get; set; }
}
