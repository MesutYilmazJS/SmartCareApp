using System.ComponentModel.DataAnnotations;

namespace SmartCareApp.Models;

public class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    [Display(Name = "Randevu zamanı")]
    public DateTimeOffset ScheduledAt { get; set; }

    [Required]
    [StringLength(200)]
    public string Reason { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    [Display(Name = "Randevu türü")]
    public string VisitType { get; set; } = string.Empty;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    [Range(0, 100)]
    [Display(Name = "Tahmini katılım riski")]
    public int PredictedNoShowRisk { get; set; }
}
