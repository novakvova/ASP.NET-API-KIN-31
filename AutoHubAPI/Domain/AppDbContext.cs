using Domain.Entities;
using Domain.Entities.TestHub;
using Microsoft.EntityFrameworkCore;

namespace Domain;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<FuelTypeEntity> FuelTypes { get; set; }
    public DbSet<VehicleStatusEntity> VehicleStatuses { get; set; }
    public DbSet<LeadStatusEntity> LeadStatuses { get; set; }
    public DbSet<BodyTypeEntity> BodyTypes { get; set; }
    public DbSet<BrandEntity> Brands { get; set; }
    public DbSet<VehicleEntity> Vehicles { get; set; }
    public DbSet<VehicleImageEntity> VehicleImages { get; set; }
    public DbSet<VehicleSpecificationEntity> VehicleSpecifications { get; set; }
    public DbSet<LeadEntity> Leads { get; set; }

    public DbSet<TestEntity> Tests => Set<TestEntity>();
    public DbSet<QuestionEntity> Questions => Set<QuestionEntity>();
    public DbSet<AnswerOptionEntity> AnswerOptions => Set<AnswerOptionEntity>();
}

