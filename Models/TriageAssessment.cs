namespace SmartCareApp.Models;

public class TriageAssessment
{
    public int RiskScore { get; init; }
    public int NoShowRiskScore { get; init; }
    public string PriorityLabel { get; init; } = string.Empty;
    public IReadOnlyList<string> Signals { get; init; } = Array.Empty<string>();
}
