using SmartCareApp.Models;

namespace SmartCareApp.Extensions;

public static class PatientCareStatusExtensions
{
    public static string ToDisplayText(this PatientCareStatus status) =>
        status switch
        {
            PatientCareStatus.New => "Yeni kayıt",
            PatientCareStatus.UnderReview => "İncelemede",
            PatientCareStatus.InTreatment => "Tedavide",
            PatientCareStatus.Stable => "Stabil",
            PatientCareStatus.Discharged => "Taburcu",
            _ => status.ToString()
        };

    public static string ToCssClass(this PatientCareStatus status) =>
        status switch
        {
            PatientCareStatus.New => "care-status-new",
            PatientCareStatus.UnderReview => "care-status-review",
            PatientCareStatus.InTreatment => "care-status-treatment",
            PatientCareStatus.Stable => "care-status-stable",
            PatientCareStatus.Discharged => "care-status-discharged",
            _ => "care-status-new"
        };
}
