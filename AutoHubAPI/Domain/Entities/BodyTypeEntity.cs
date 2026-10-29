using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

/// <summary>Тип кузова: Бортовий, Рефрижератор, Тент, Фургон, Дубль-кабіна борт, Самоскид, Евакуатор, Платформа.</summary>
[Table("body_types")]
[Index(nameof(Slug), IsUnique = true)]
public class BodyTypeEntity : BaseEntity<int>
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>URL-частина для фільтра/меню, напр. "dubl-kabina-bort".</summary>
    [Required, MaxLength(100)]
    public string Slug { get; set; } = null!;

    /// <summary>Картинка плитки у фільтрі.</summary>
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public int SortOrder { get; set; }

    public ICollection<VehicleEntity> Vehicles { get; set; } = new List<VehicleEntity>();
}
