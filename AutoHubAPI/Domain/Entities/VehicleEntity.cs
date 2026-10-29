using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

// ───────────────────────── Автомобіль ─────────────────────────

[Table("vehicles")]
[Index(nameof(BrandId))]
[Index(nameof(BodyTypeId))]
[Index(nameof(Price))]
[Index(nameof(Year))]
[Index(nameof(FuelTypeId))]
[Index(nameof(StatusId), nameof(CreatedAt))]
public class VehicleEntity : BaseEntity<int>
{
    [ForeignKey(nameof(Brand))]
    public int BrandId { get; set; }
    public BrandEntity Brand { get; set; } = null!;

    [ForeignKey(nameof(BodyType))]
    public int BodyTypeId { get; set; }
    public BodyTypeEntity BodyType { get; set; } = null!;

    /// <summary>Модель: Crafter, Sprinter...</summary>
    [Required, MaxLength(100)]
    public string Model { get; set; } = null!;

    /// <summary>Підзаголовок картки, напр. "Комерційний автомобіль".</summary>
    [MaxLength(150)]
    public string? Subtitle { get; set; }

    [Range(1980, 2100)]
    public int Year { get; set; }

    /// <summary>Пробіг, км.</summary>
    [Range(0, 5_000_000)]
    public int Mileage { get; set; }

    /// <summary>Об'єм двигуна, л (напр. 2.0).</summary>
    [Column(TypeName = "numeric(3,1)")]
    [Range(0.1, 20)]
    public decimal EngineVolume { get; set; }

    [ForeignKey(nameof(FuelType))]
    public int FuelTypeId { get; set; }
    public FuelTypeEntity FuelType { get; set; } = null!;

    /// <summary>Ціна в USD.</summary>
    [Column(TypeName = "numeric(12,2)")]
    [Range(0, 100_000_000)]
    public decimal Price { get; set; }

    [MaxLength(17)]
    public string? Vin { get; set; }

    public string? Description { get; set; }

    /// <summary>За замовчуванням 1 = "В наявності".</summary>
    [ForeignKey(nameof(Status))]
    public int StatusId { get; set; } = 1;
    public VehicleStatusEntity Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<VehicleImageEntity> Images { get; set; } = new List<VehicleImageEntity>();
    public ICollection<VehicleSpecificationEntity> Specifications { get; set; } = new List<VehicleSpecificationEntity>();
    public ICollection<LeadEntity> Leads { get; set; } = new List<LeadEntity>();
}