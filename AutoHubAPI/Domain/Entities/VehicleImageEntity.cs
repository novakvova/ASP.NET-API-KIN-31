using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Entities;

/// <summary>Фото авто (галерея на сторінці "Детальніше").</summary>
[Table("vehicle_images")]
[Index(nameof(VehicleId), nameof(SortOrder))]
public class VehicleImageEntity : BaseEntity<int>
{
    [ForeignKey(nameof(Vehicle))]
    public int VehicleId { get; set; }
    public VehicleEntity Vehicle { get; set; } = null!;

    [Required, MaxLength(500)]
    public string Url { get; set; } = null!;

    /// <summary>Головне фото для картки в каталозі.</summary>
    public bool IsMain { get; set; }

    public int SortOrder { get; set; }
}
