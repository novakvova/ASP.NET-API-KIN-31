using Application.Interfaces;
using Application.Models.Tests;
using Domain;
using Domain.Entities.TestHub;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class TestService(AppDbContext db) : ITestService
{
    public async Task<IReadOnlyList<TestItemDto>> GetAllAsync()
    {
        return await db.Tests
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .Select(t => new TestItemDto(
                t.Id, t.Title, t.Description, t.TimeLimitMinutes, t.CreatedAt, t.Questions.Count))
            .ToListAsync();
    }

    public async Task<TestItemDto?> GetByIdAsync(int id)
    {
        return await db.Tests
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TestItemDto(
                t.Id, t.Title, t.Description, t.TimeLimitMinutes, t.CreatedAt, t.Questions.Count))
            .FirstOrDefaultAsync();
    }

    public async Task<TestItemDto> CreateAsync(TestSaveRequest request)
    {
        var entity = new TestEntity
        {
            Title = request.Title.Trim(),
            Description = request.Description,
            TimeLimitMinutes = request.TimeLimitMinutes,
            CreatedAt = DateTime.UtcNow
        };

        db.Tests.Add(entity);
        await db.SaveChangesAsync();

        return new TestItemDto(entity.Id, entity.Title, entity.Description,
            entity.TimeLimitMinutes, entity.CreatedAt, 0);
    }

    public async Task<TestItemDto?> UpdateAsync(int id, TestSaveRequest request)
    {
        var entity = await db.Tests.FindAsync(id);
        if (entity == null)
            return null;

        entity.Title = request.Title.Trim();
        entity.Description = request.Description;
        entity.TimeLimitMinutes = request.TimeLimitMinutes;
        await db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    /// <summary>Повне видалення тесту (питання й відповіді видаляються каскадно).</summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await db.Tests.FindAsync(id);
        if (entity == null)
            return false;

        db.Tests.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<TestInfoDto?> GetInfoAsync(int id)
    {
        return await db.Tests
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TestInfoDto(
                t.Id, t.Title, t.Description, t.TimeLimitMinutes, t.CreatedAt,
                t.Questions.Count,
                t.Questions.Sum(q => q.Points)))
            .FirstOrDefaultAsync();
    }

    public async Task<TestPassDto?> GetForPassingAsync(int id)
    {
        return await db.Tests
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TestPassDto(
                t.Id, t.Title, t.TimeLimitMinutes,
                t.Questions
                    .OrderBy(q => q.OrderIndex).ThenBy(q => q.Id)
                    .Select(q => new QuestionPassDto(
                        q.Id, q.Text, q.OrderIndex, q.Points,
                        q.AnswerOptions.Count(a => a.IsCorrect) > 1,
                        q.AnswerOptions
                            .OrderBy(a => a.Id)
                            .Select(a => new AnswerOptionPassDto(a.Id, a.Text))
                            .ToList()))
                    .ToList()))
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Питання зараховується повністю, лише якщо обрано рівно всі правильні варіанти
    /// (без зайвих). Часткових балів немає. Варіанти, що не належать питанню, ігноруються.
    /// </summary>
    public async Task<TestResultDto?> SubmitAsync(int id, TestSubmitRequest request)
    {
        var test = await db.Tests
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new
            {
                t.Id,
                t.Title,
                Questions = t.Questions
                    .OrderBy(q => q.OrderIndex).ThenBy(q => q.Id)
                    .Select(q => new
                    {
                        q.Id,
                        q.Text,
                        q.Points,
                        OptionIds = q.AnswerOptions.Select(a => a.Id).ToList(),
                        CorrectIds = q.AnswerOptions.Where(a => a.IsCorrect).Select(a => a.Id).ToList()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (test == null)
            return null;

        // Якщо одне питання надіслано кілька разів — об'єднуємо вибрані варіанти
        var selectedByQuestion = request.Answers
            .GroupBy(a => a.QuestionId)
            .ToDictionary(g => g.Key, g => g.SelectMany(a => a.SelectedOptionIds).ToHashSet());

        var results = test.Questions.Select(q =>
        {
            var selected = selectedByQuestion.TryGetValue(q.Id, out var ids)
                ? ids.Where(q.OptionIds.Contains).OrderBy(x => x).ToList()
                : new List<int>();

            var isCorrect = q.CorrectIds.Count > 0 && selected.ToHashSet().SetEquals(q.CorrectIds);

            return new QuestionResultDto(
                q.Id, q.Text, q.Points,
                isCorrect ? q.Points : 0,
                isCorrect,
                selected,
                q.CorrectIds.OrderBy(x => x).ToList());
        }).ToList();

        var score = results.Sum(r => r.EarnedPoints);
        var maxScore = results.Sum(r => r.Points);
        var percent = maxScore == 0 ? 0 : Math.Round(score * 100.0 / maxScore, 1);

        return new TestResultDto(
            test.Id, test.Title, score, maxScore, percent,
            results.Count(r => r.IsCorrect), results.Count, results);
    }
}
