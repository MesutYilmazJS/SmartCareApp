using System.ComponentModel.DataAnnotations;

namespace SmartCareApp.Models;

public class MedicalHistory
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public DateTimeOffset RecordedAt { get; set; }

    [Required]
    [StringLength(60)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;

    public bool ClinicallyRelevant { get; set; }
}
