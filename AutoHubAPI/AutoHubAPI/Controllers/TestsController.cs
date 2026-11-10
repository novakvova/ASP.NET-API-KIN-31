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
}
