namespace Application.Interfaces;

public interface ISeederService
{
    public Task UpdateDatabase();
    public Task SeedLeadStatusesAsync();
    public Task SeedFuelTypesAsync();
    public Task SeedBodyTypesAsync();
    public Task SeedBrandsAsync();
    public Task SeedVehiclesAsync();
    public Task SeedVehicleStatusesAsync();

    public Task SeedTestsAsync();
    public Task SeedQuestionsAsync();
    public Task SeedAnswerOptionsAsync();
}
