using Application.Interfaces;
using Application.Models.Tests;
using Microsoft.AspNetCore.Mvc;

namespace AutoHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestsController(ITestService testService) : ControllerBase
{
    /// <summary>Список усіх тестів у БД.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TestItemDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var tests = await testService.GetAllAsync();
        return Ok(tests);
    }

    /// <summary>Інформація про тест (кількість питань, максимальний бал, ліміт часу).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<TestInfoDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInfo(int id)
    {
        var info = await testService.GetInfoAsync(id);
        return info == null ? NotFound() : Ok(info);
    }

    /// <summary>Питання тесту з варіантами відповідей для проходження (без правильних відповідей).</summary>
    [HttpGet("{id:int}/pass")]
    [ProducesResponseType<TestPassDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetForPassing(int id)
    {
        var test = await testService.GetForPassingAsync(id);
        return test == null ? NotFound() : Ok(test);
    }

    /// <summary>Надіслати відповіді та отримати результат.</summary>
    [HttpPost("{id:int}/submit")]
    [ProducesResponseType<TestResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Submit(int id, [FromBody] TestSubmitRequest request)
    {
        var result = await testService.SubmitAsync(id, request);
        return result == null ? NotFound() : Ok(result);
    }
}
