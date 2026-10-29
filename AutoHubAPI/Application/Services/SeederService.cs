using Application.Interfaces;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class SeederService(AppDbContext db)
    : ISeederService
{
    public async Task SeedVehicleStatusesAsync()
    {
        if (await db.VehicleStatuses.AnyAsync())
            return;

        var vStatuses = new List<VehicleStatusEntity>
        {
            new ()  { Name = "В наявності" },
            new ()  { Name = "Заброньовано" },
            new ()  { Name = "Продано" }
        };
        await db.VehicleStatuses.AddRangeAsync(vStatuses);
        await db.SaveChangesAsync();
    }

    public async Task UpdateDatabase()
    {
        await db.Database.MigrateAsync();
    }
}
