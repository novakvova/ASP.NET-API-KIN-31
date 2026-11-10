using Application.Models.Tests;

namespace Application.Interfaces;

public interface ITestService
{
    Task<IReadOnlyList<TestItemDto>> GetAllAsync();
    Task<TestItemDto?> GetByIdAsync(int id);
    Task<TestItemDto> CreateAsync(TestSaveRequest request);
    Task<TestItemDto?> UpdateAsync(int id, TestSaveRequest request);
    Task<bool> DeleteAsync(int id);
}
