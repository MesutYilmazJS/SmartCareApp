using SmartCareApp.Models;

namespace SmartCareApp.ViewModels;

public class PatientDetailsViewModel
{
    public Patient Patient { get; init; } = null!;
    public TriageAssessment Assessment { get; init; } = null!;
    public IReadOnlyList<Appointment> UpcomingAppointments { get; init; } = Array.Empty<Appointment>();
    public string ProfileImageUrl { get; init; } = string.Empty;
}
