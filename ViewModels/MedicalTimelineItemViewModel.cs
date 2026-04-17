namespace SmartCareApp.ViewModels;

public class MedicalTimelineItemViewModel
{
    public DateTimeOffset OccurredAt { get; init; }
    public string Category { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Detail { get; init; } = string.Empty;
    public string Tone { get; init; } = "neutral";
}
