using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>Тип палива: 
/// Дизель, Бензин, Газ, Газ/бензин, Гібрид, Електро.
/// </summary>
[Table("fuel_types")]
[Index(nameof(Name), IsUnique = true)]
public class FuelTypeEntity : BaseEntity<int>
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;
    public ICollection<VehicleEntity> Vehicles { get; set; } = new List<VehicleEntity>();
}
