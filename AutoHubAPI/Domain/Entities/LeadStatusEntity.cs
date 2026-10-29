using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>Статус заявки: Нова, В роботі, Виконана, Відхилена.</summary>
[Table("lead_statuses")]
[Index(nameof(Name), IsUnique = true)]
public class LeadStatusEntity : BaseEntity<int>
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;

    public ICollection<LeadEntity> Leads { get; set; } = new List<LeadEntity>();
}