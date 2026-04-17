namespace SmartCareApp.ViewModels;

public class DashboardViewModel
{
    public int ActivePatientCount { get; init; }
    public int UrgentQueueCount { get; init; }
    public int NoShowAlertCount { get; init; }
    public int TodayAppointmentCount { get; init; }
    public IReadOnlyList<DashboardPatientItemViewModel> PriorityQueue { get; init; } = Array.Empty<DashboardPatientItemViewModel>();
    public IReadOnlyList<DashboardInsightViewModel> Insights { get; init; } = Array.Empty<DashboardInsightViewModel>();
    public IReadOnlyList<DoctorLoadViewModel> DoctorLoads { get; init; } = Array.Empty<DoctorLoadViewModel>();
    public IReadOnlyList<DashboardAppointmentItemViewModel> UpcomingAppointments { get; init; } = Array.Empty<DashboardAppointmentItemViewModel>();
}

public class DashboardPatientItemViewModel
{
    public int PatientId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public int Age { get; init; }
    public string PrimarySymptom { get; init; } = string.Empty;
    public int RiskScore { get; init; }
    public int NoShowRiskScore { get; init; }
    public string ChronicConditions { get; init; } = string.Empty;
    public string ProfileImageUrl { get; init; } = string.Empty;
}

public class DashboardInsightViewModel
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Accent { get; init; } = string.Empty;
}

public class DoctorLoadViewModel
{
    public string FullName { get; init; } = string.Empty;
    public string Specialty { get; init; } = string.Empty;
    public string RoomNumber { get; init; } = string.Empty;
    public int AppointmentCount { get; init; }
    public string ProfileImageUrl { get; init; } = string.Empty;
}

public class DashboardAppointmentItemViewModel
{
    public string PatientName { get; init; } = string.Empty;
    public string DoctorName { get; init; } = string.Empty;
    public DateTimeOffset ScheduledAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public int NoShowRiskScore { get; init; }
    public string PatientProfileImageUrl { get; init; } = string.Empty;
}
