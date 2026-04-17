using System.Globalization;
using System.Text;
using SmartCareApp.Models;

namespace SmartCareApp.Services;

public class TriageRiskService : ITriageRiskService
{
    public TriageAssessment Assess(Patient patient)
    {
        var signals = new List<string>();
        var riskScore = 12;

        if (patient.Age >= 65)
        {
            riskScore += 18;
            signals.Add("65 yaş üzeri izlem");
        }
        else if (patient.Age >= 50)
        {
            riskScore += 10;
            signals.Add("Yaşa bağlı yakın takip");
        }

        var symptom = NormalizeForComparison(patient.PrimarySymptom);
        AddSignalIfMatch(symptom, "gogus", 28, "Göğüs ağrısı bildirimi", signals, ref riskScore);
        AddSignalIfMatch(symptom, "nefes", 24, "Nefes darlığı belirtisi", signals, ref riskScore);
        AddSignalIfMatch(symptom, "ates", 10, "Enfeksiyon bulgusu", signals, ref riskScore);
        AddSignalIfMatch(symptom, "oksur", 8, "Solunum yakınması", signals, ref riskScore);
        AddSignalIfMatch(symptom, "bas don", 10, "Baş dönmesi artışı", signals, ref riskScore);
        AddSignalIfMatch(symptom, "migren", 8, "Nörolojik yakınma", signals, ref riskScore);
        AddSignalIfMatch(symptom, "yorgun", 6, "Genel halsizlik", signals, ref riskScore);

        if (patient.HasCardiovascularDisease)
        {
            riskScore += 18;
            signals.Add("Kardiyovasküler öykü");
        }

        if (patient.HasHypertension)
        {
            riskScore += 10;
            signals.Add("Hipertansiyon öyküsü");
        }

        if (patient.HasDiabetes)
        {
            riskScore += 10;
            signals.Add("Diyabet öyküsü");
        }

        if (!string.IsNullOrWhiteSpace(patient.ChronicConditions))
        {
            riskScore += 6;
            signals.Add("Ek hastalık notu var");
        }

        riskScore = Math.Clamp(riskScore, 0, 100);

        var noShowRiskScore = 10 + (patient.MissedAppointmentCount * 18);

        if (patient.PortalLoginCountLast30Days == 0)
        {
            noShowRiskScore += 24;
            signals.Add("Son dönemde giriş hareketi yok");
        }
        else if (patient.PortalLoginCountLast30Days <= 2)
        {
            noShowRiskScore += 10;
            signals.Add("Giriş hareketi düşük");
        }

        if (riskScore >= 75)
        {
            noShowRiskScore -= 8;
        }

        noShowRiskScore = Math.Clamp(noShowRiskScore, 0, 100);

        return new TriageAssessment
        {
            RiskScore = riskScore,
            NoShowRiskScore = noShowRiskScore,
            PriorityLabel = GetPriorityLabel(riskScore),
            Signals = signals.Distinct().ToList()
        };
    }

    private static string GetPriorityLabel(int score) =>
        score switch
        {
            >= 80 => "Kritik izlem",
            >= 65 => "Öncelikli değerlendirme",
            >= 45 => "Yakın takip",
            _ => "Rutin izlem"
        };

    private static void AddSignalIfMatch(
        string symptom,
        string term,
        int weight,
        string label,
        ICollection<string> signals,
        ref int riskScore)
    {
        if (!symptom.Contains(term, StringComparison.Ordinal))
        {
            return;
        }

        riskScore += weight;
        signals.Add(label);
    }

    private static string NormalizeForComparison(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.ToLower(new CultureInfo("tr-TR")).Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace('ı', 'i');
    }
}
