using SmartCareApp.Models;

namespace SmartCareApp.Extensions;

public static class AppointmentStatusExtensions
{
    public static string ToDisplayText(this AppointmentStatus status) =>
        status switch
        {
            AppointmentStatus.Scheduled => "Planlandı",
            AppointmentStatus.InProgress => "İşlemde",
            AppointmentStatus.Completed => "Tamamlandı",
            AppointmentStatus.Cancelled => "İptal",
            AppointmentStatus.NoShow => "Gelmedi",
            _ => status.ToString()
        };
}
