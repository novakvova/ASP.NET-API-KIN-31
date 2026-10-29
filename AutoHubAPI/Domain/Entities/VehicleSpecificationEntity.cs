using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

/// <summary>Додаткові характеристики (коробка передач, потужність, колісна база тощо) у вигляді ключ-значення.</summary>
[Table("vehicle_specifications")]
[Index(nameof(VehicleId))]
public class VehicleSpecificationEntity : BaseEntity<int>
{
    [ForeignKey(nameof(Vehicle))]
    public int VehicleId { get; set; }
    public VehicleEntity Vehicle { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(300)]
    public string Value { get; set; } = null!;

    public int SortOrder { get; set; }

}
