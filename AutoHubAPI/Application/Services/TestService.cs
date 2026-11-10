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
}
