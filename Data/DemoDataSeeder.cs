using Microsoft.EntityFrameworkCore;
using SmartCareApp.Models;
using SmartCareApp.Services;

namespace SmartCareApp.Data;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext, ITriageRiskService triageRiskService)
    {
        if (!await dbContext.AdminAccessCodes.AnyAsync())
        {
            await dbContext.AdminAccessCodes.AddAsync(new AdminAccess
            {
                AccessCode = "1234"
            });

            await dbContext.SaveChangesAsync();
        }

        if (await dbContext.Patients.AnyAsync())
        {
            return;
        }

        var doctors = new List<Doctor>
        {
            new() { FullName = "Dr. Melis Aydın", Specialty = "Kardiyoloji", RoomNumber = "A-12", Username = "melis", Password = "doktor123", ProfileImageFileName = "kadın1.jpg" },
            new() { FullName = "Dr. Bora Sancak", Specialty = "İç Hastalıkları", RoomNumber = "B-05", Username = "bora", Password = "doktor123", ProfileImageFileName = "erkek1.webp" },
            new() { FullName = "Dr. Ece Demir", Specialty = "Göğüs Hastalıkları", RoomNumber = "C-02", Username = "ece", Password = "doktor123", ProfileImageFileName = "kadın2.webp" },
            new() { FullName = "Dr. Deniz Öztürk", Specialty = "Nöroloji", RoomNumber = "D-03", Username = "deniz", Password = "doktor123", ProfileImageFileName = "erkek2.jpg" }
        };

        var patients = new List<Patient>
        {
            new()
            {
                FullName = "Ayşe Kalkan",
                NationalId = "10000000001",
                DateOfBirth = new DateOnly(1958, 11, 12),
                PrimarySymptom = "Göğüs baskısı ve nefes darlığı",
                ChronicConditions = "Hipertansiyon, anjiyoplasti öyküsü",
                HasHypertension = true,
                HasCardiovascularDisease = true,
                MissedAppointmentCount = 0,
                PortalLoginCountLast30Days = 4,
                IntakeNotes = "Evde ölçülen yüksek tansiyon sonrası yakın izleme alındı.",
                AccessCode = "H910001",
                ProfileImageFileName = "kadın1.jpg",
                CareStatus = PatientCareStatus.InTreatment,
                CreatedByDoctor = doctors[0]
            },
            new()
            {
                FullName = "Mert Çağlar",
                NationalId = "10000000002",
                DateOfBirth = new DateOnly(1989, 3, 7),
                PrimarySymptom = "Uzayan ateş, öksürük ve yorgunluk",
                ChronicConditions = "Tip 2 diyabet",
                HasDiabetes = true,
                MissedAppointmentCount = 1,
                PortalLoginCountLast30Days = 1,
                IntakeNotes = "Eksik kalan tedavi sonrası yeniden değerlendirmeye alındı.",
                AccessCode = "H910002",
                ProfileImageFileName = "erkek1.jpg",
                CareStatus = PatientCareStatus.UnderReview,
                CreatedByDoctor = doctors[1]
            },
            new()
            {
                FullName = "Selin Arslan",
                NationalId = "10000000003",
                DateOfBirth = new DateOnly(1997, 8, 20),
                PrimarySymptom = "Migren ve baş dönmesi",
                ChronicConditions = "Mevsimsel alerji",
                MissedAppointmentCount = 2,
                PortalLoginCountLast30Days = 0,
                IntakeNotes = "İş yerindeki atak sonrası aynı gün değerlendirme istendi.",
                AccessCode = "H910003",
                ProfileImageFileName = "kadın1.jpg",
                CareStatus = PatientCareStatus.UnderReview,
                CreatedByDoctor = doctors[3]
            },
            new()
            {
                FullName = "Cemre Taş",
                NationalId = "10000000004",
                DateOfBirth = new DateOnly(1974, 6, 14),
                PrimarySymptom = "Nefes darlığı ve kuru öksürük",
                ChronicConditions = "Astım, hipertansiyon",
                HasHypertension = true,
                MissedAppointmentCount = 0,
                PortalLoginCountLast30Days = 6,
                IntakeNotes = "Son iki gecedir artan yakınmalar nedeniyle hızlı kontrole alındı.",
                AccessCode = "H910004",
                ProfileImageFileName = "erkek2.webp",
                CareStatus = PatientCareStatus.InTreatment,
                CreatedByDoctor = doctors[2]
            },
            new()
            {
                FullName = "İlker Yılmaz",
                NationalId = "10000000005",
                DateOfBirth = new DateOnly(1966, 1, 30),
                PrimarySymptom = "Baş dönmesi ve halsizlik",
                ChronicConditions = "Prediyabet",
                HasDiabetes = true,
                MissedAppointmentCount = 1,
                PortalLoginCountLast30Days = 3,
                IntakeNotes = "Sabah yaşanan kısa süreli denge kaybı sonrası gözlem önerildi.",
                AccessCode = "H910005",
                ProfileImageFileName = "erkek3.jpg",
                CareStatus = PatientCareStatus.Stable,
                CreatedByDoctor = doctors[3]
            },
            new()
            {
                FullName = "Zeynep Karaca",
                NationalId = "10000000006",
                DateOfBirth = new DateOnly(2001, 12, 4),
                PrimarySymptom = "Yüksek ateş ve boğaz ağrısı",
                ChronicConditions = string.Empty,
                MissedAppointmentCount = 0,
                PortalLoginCountLast30Days = 2,
                IntakeNotes = "Aynı gün içinde ikinci kez başvurduğu için ön sıralamaya alındı.",
                AccessCode = "H910006",
                ProfileImageFileName = "kadın1.jpg",
                CareStatus = PatientCareStatus.New,
                CreatedByDoctor = doctors[1]
            }
        };

        foreach (var patient in patients)
        {
            ApplyAssessment(patient, triageRiskService);
        }

        await dbContext.Doctors.AddRangeAsync(doctors);
        await dbContext.Patients.AddRangeAsync(patients);
        await dbContext.SaveChangesAsync();

        var appointments = new List<Appointment>
        {
            new()
            {
                PatientId = patients[0].Id,
                DoctorId = doctors[0].Id,
                ScheduledAt = DateTimeOffset.UtcNow.AddHours(2),
                Status = AppointmentStatus.Scheduled,
                Reason = "Tansiyon dengesi ve kardiyak yakınma kontrolü",
                VisitType = "Öncelikli değerlendirme",
                PredictedNoShowRisk = patients[0].NoShowRiskScore
            },
            new()
            {
                PatientId = patients[1].Id,
                DoctorId = doctors[2].Id,
                ScheduledAt = DateTimeOffset.UtcNow.AddHours(5),
                Status = AppointmentStatus.Scheduled,
                Reason = "Solunum yakınmalarının değerlendirilmesi",
                VisitType = "Aynı gün başvuru",
                PredictedNoShowRisk = patients[1].NoShowRiskScore
            },
            new()
            {
                PatientId = patients[2].Id,
                DoctorId = doctors[3].Id,
                ScheduledAt = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
                Status = AppointmentStatus.Scheduled,
                Reason = "Nörolojik tarama ve ilk değerlendirme",
                VisitType = "Konsültasyon",
                PredictedNoShowRisk = patients[2].NoShowRiskScore
            },
            new()
            {
                PatientId = patients[3].Id,
                DoctorId = doctors[2].Id,
                ScheduledAt = DateTimeOffset.UtcNow.AddHours(1),
                Status = AppointmentStatus.InProgress,
                Reason = "Solunum fonksiyonu ve oksijen takibi",
                VisitType = "Yakın takip",
                PredictedNoShowRisk = patients[3].NoShowRiskScore
            },
            new()
            {
                PatientId = patients[4].Id,
                DoctorId = doctors[1].Id,
                ScheduledAt = DateTimeOffset.UtcNow.AddHours(7),
                Status = AppointmentStatus.Scheduled,
                Reason = "Denge kaybı ve halsizlik değerlendirmesi",
                VisitType = "Poliklinik kontrolü",
                PredictedNoShowRisk = patients[4].NoShowRiskScore
            },
            new()
            {
                PatientId = patients[5].Id,
                DoctorId = doctors[1].Id,
                ScheduledAt = DateTimeOffset.UtcNow.AddHours(4),
                Status = AppointmentStatus.Scheduled,
                Reason = "Ateş takibi ve laboratuvar değerlendirmesi",
                VisitType = "Hızlı değerlendirme",
                PredictedNoShowRisk = patients[5].NoShowRiskScore
            },
            new()
            {
                PatientId = patients[0].Id,
                DoctorId = doctors[0].Id,
                ScheduledAt = DateTimeOffset.UtcNow.AddDays(-12),
                Status = AppointmentStatus.Completed,
                Reason = "Acil yönlendirme sonrası kontrol",
                VisitType = "Doğrudan başvuru",
                PredictedNoShowRisk = 10
            },
            new()
            {
                PatientId = patients[1].Id,
                DoctorId = doctors[1].Id,
                ScheduledAt = DateTimeOffset.UtcNow.AddDays(-20),
                Status = AppointmentStatus.NoShow,
                Reason = "Rutin diyabet kontrolü",
                VisitType = "Rutin kontrol",
                PredictedNoShowRisk = 62
            },
            new()
            {
                PatientId = patients[3].Id,
                DoctorId = doctors[2].Id,
                ScheduledAt = DateTimeOffset.UtcNow.AddDays(-2),
                Status = AppointmentStatus.Completed,
                Reason = "Nebül tedavisi sonrası solunum kontrolü",
                VisitType = "Takip görüşmesi",
                PredictedNoShowRisk = 18
            },
            new()
            {
                PatientId = patients[4].Id,
                DoctorId = doctors[3].Id,
                ScheduledAt = DateTimeOffset.UtcNow.AddDays(-1),
                Status = AppointmentStatus.Cancelled,
                Reason = "Baş dönmesi şikayetinde yeniden planlama",
                VisitType = "Kontrol randevusu",
                PredictedNoShowRisk = 44
            }
        };

        var medicalHistories = new List<MedicalHistory>
        {
            new()
            {
                PatientId = patients[0].Id,
                RecordedAt = DateTimeOffset.UtcNow.AddDays(-12),
                Category = "Değerlendirme",
                Title = "Acil hipertansiyon atağı",
                Notes = "Gözlem sonrası tansiyon dengelendi. Kardiyoloji kontrolü önerildi.",
                ClinicallyRelevant = true
            },
            new()
            {
                PatientId = patients[0].Id,
                RecordedAt = DateTimeOffset.UtcNow.AddDays(-5),
                Category = "Tedavi",
                Title = "İlaç dozu güncellendi",
                Notes = "Eforla artan göğüs sıkışması nedeniyle doz artırıldı.",
                ClinicallyRelevant = true
            },
            new()
            {
                PatientId = patients[1].Id,
                RecordedAt = DateTimeOffset.UtcNow.AddDays(-20),
                Category = "Katılım",
                Title = "Kaçırılan diyabet kontrolü",
                Notes = "Hasta randevuya gelmedi, aynı hafta içinde giriş hareketi izlenmedi.",
                ClinicallyRelevant = false
            },
            new()
            {
                PatientId = patients[1].Id,
                RecordedAt = DateTimeOffset.UtcNow.AddDays(-3),
                Category = "Laboratuvar",
                Title = "CRP ve kan şekeri istendi",
                Notes = "Reçete yenilenmeden önce iç hastalıkları değerlendirmesine alındı.",
                ClinicallyRelevant = true
            },
            new()
            {
                PatientId = patients[2].Id,
                RecordedAt = DateTimeOffset.UtcNow.AddDays(-1),
                Category = "Başvuru",
                Title = "Baş ağrısı şiddeti arttı",
                Notes = "Uzun süreli ekran maruziyeti sonrası baş dönmesi artınca gözlem istendi.",
                ClinicallyRelevant = true
            },
            new()
            {
                PatientId = patients[3].Id,
                RecordedAt = DateTimeOffset.UtcNow.AddDays(-2),
                Category = "Tedavi",
                Title = "Nebül uygulaması tamamlandı",
                Notes = "Gece artan öksürük sonrası rahatlama sağlandı, yakın takip planlandı.",
                ClinicallyRelevant = true
            },
            new()
            {
                PatientId = patients[4].Id,
                RecordedAt = DateTimeOffset.UtcNow.AddDays(-1),
                Category = "Gözlem",
                Title = "Kısa süreli denge kaybı izlendi",
                Notes = "Şikayet tekrarlamadı ancak nörolojik kontrol öne çekildi.",
                ClinicallyRelevant = true
            },
            new()
            {
                PatientId = patients[5].Id,
                RecordedAt = DateTimeOffset.UtcNow.AddHours(-14),
                Category = "Laboratuvar",
                Title = "Hızlı test paneli istendi",
                Notes = "Ateş yüksek seyretmeye devam ettiği için örnekler öncelikli işlendi.",
                ClinicallyRelevant = true
            }
        };

        await dbContext.Appointments.AddRangeAsync(appointments);
        await dbContext.MedicalHistories.AddRangeAsync(medicalHistories);
        await dbContext.SaveChangesAsync();
    }

    private static void ApplyAssessment(Patient patient, ITriageRiskService triageRiskService)
    {
        var assessment = triageRiskService.Assess(patient);
        patient.RiskScore = assessment.RiskScore;
        patient.NoShowRiskScore = assessment.NoShowRiskScore;
    }
}
