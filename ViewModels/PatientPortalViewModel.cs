using SmartCareApp.Models;

namespace SmartCareApp.ViewModels;

public class PatientPortalViewModel
{
    public Patient Patient { get; init; } = null!;
    public TriageAssessment Assessment { get; init; } = null!;
    public string ProfileImageUrl { get; init; } = string.Empty;
    public IReadOnlyList<ProfileImageOptionViewModel> ImageOptions { get; init; } = Array.Empty<ProfileImageOptionViewModel>();
    public IReadOnlyList<Appointment> UpcomingAppointments { get; init; } = Array.Empty<Appointment>();
}
