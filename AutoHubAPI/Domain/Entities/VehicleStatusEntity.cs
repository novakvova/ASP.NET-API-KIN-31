
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>Статус авто: В наявності, Заброньовано, Продано.</summary>
[Table("vehicle_statuses")]
[Index(nameof(Name), IsUnique = true)]
public class VehicleStatusEntity : BaseEntity<int>
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;
    public ICollection<VehicleEntity> Vehicles { get; set; } = new List<VehicleEntity>();
}
