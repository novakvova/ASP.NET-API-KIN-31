using Application.Interfaces;
using Quartz;

namespace Infrastructure.Jobs;

public class DbSeedJob(ISeederService seederService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await seederService.UpdateDatabase();
        await seederService.SeedTestsAsync();
        await seederService.SeedQuestionsAsync();
        await seederService.SeedAnswerOptionsAsync();
    }
}
