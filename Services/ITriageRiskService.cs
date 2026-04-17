using SmartCareApp.Models;

namespace SmartCareApp.Services;

public interface ITriageRiskService
{
    TriageAssessment Assess(Patient patient);
}
