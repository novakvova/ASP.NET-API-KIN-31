using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

/// <summary>Заявка / запит на дзвінок з сайту (кнопка "Зателефонувати").</summary>
[Table("leads")]
[Index(nameof(StatusId), nameof(CreatedAt))]
public class LeadEntity : BaseEntity<int>
{
    /// <summary>Авто, за яким залишили заявку (може бути null для загальної).</summary>
    [ForeignKey(nameof(Vehicle))]
    public int? VehicleId { get; set; }
    public VehicleEntity? Vehicle { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(32), Phone]
    public string Phone { get; set; } = null!;

    [MaxLength(1000)]
    public string? Message { get; set; }

    /// <summary>За замовчуванням 1 = "Нова".</summary>
    [ForeignKey(nameof(Status))]
    public int StatusId { get; set; } = 1;
    public LeadStatusEntity Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
