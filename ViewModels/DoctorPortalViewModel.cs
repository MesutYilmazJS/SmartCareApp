using SmartCareApp.Models;

namespace SmartCareApp.ViewModels;

public class DoctorPortalViewModel
{
    public Doctor Doctor { get; init; } = null!;
    public string ProfileImageUrl { get; init; } = string.Empty;
    public IReadOnlyList<ProfileImageOptionViewModel> ImageOptions { get; init; } = Array.Empty<ProfileImageOptionViewModel>();
    public IReadOnlyList<DoctorPatientRowViewModel> Patients { get; init; } = Array.Empty<DoctorPatientRowViewModel>();
    public IReadOnlyList<Appointment> UpcomingAppointments { get; init; } = Array.Empty<Appointment>();
}

public class DoctorPatientRowViewModel
{
    public int Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public int Age { get; init; }
    public string NationalId { get; init; } = string.Empty;
    public string AccessCode { get; init; } = string.Empty;
    public string PrimarySymptom { get; init; } = string.Empty;
    public string ProfileImageUrl { get; init; } = string.Empty;
    public int RiskScore { get; init; }
    public int NoShowRiskScore { get; init; }
    public PatientCareStatus CareStatus { get; init; }
    public string LastVisitText { get; init; } = string.Empty;
}
