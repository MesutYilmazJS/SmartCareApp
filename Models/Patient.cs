using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCareApp.Models;

public class Patient
{
    public int Id { get; set; }

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [StringLength(120, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Ad soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} 11 haneli olmalıdır.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "{0} sadece rakamlardan oluşmalıdır.")]
    [Display(Name = "T.C. kimlik no")]
    public string NationalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [Display(Name = "Doğum tarihi")]
    [DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; }

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [StringLength(160, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Başvuru nedeni")]
    public string PrimarySymptom { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Ek hastalıklar")]
    public string ChronicConditions { get; set; } = string.Empty;

    [Display(Name = "Diyabet öyküsü")]
    public bool HasDiabetes { get; set; }

    [Display(Name = "Hipertansiyon öyküsü")]
    public bool HasHypertension { get; set; }

    [Display(Name = "Kardiyovasküler öykü")]
    public bool HasCardiovascularDisease { get; set; }

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [Range(0, 20, ErrorMessage = "{0} 0 ile {2} arasında olmalıdır.")]
    [Display(Name = "Kaçırılan randevu")]
    public int MissedAppointmentCount { get; set; }

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [Range(0, 30, ErrorMessage = "{0} 0 ile {2} arasında olmalıdır.")]
    [Display(Name = "Son 30 gündeki giriş sayısı")]
    public int PortalLoginCountLast30Days { get; set; }

    [Range(0, 100)]
    public int RiskScore { get; set; }

    [Range(0, 100)]
    public int NoShowRiskScore { get; set; }

    [StringLength(500, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Kayıt notu")]
    public string IntakeNotes { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Giriş kodu")]
    public string AccessCode { get; set; } = string.Empty;

    [StringLength(120, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Profil görseli")]
    public string ProfileImageFileName { get; set; } = string.Empty;

    [Display(Name = "Durum")]
    public PatientCareStatus CareStatus { get; set; } = PatientCareStatus.New;

    public int? CreatedByDoctorId { get; set; }
    public Doctor? CreatedByDoctor { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();

    [NotMapped]
    public int Age
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - DateOfBirth.Year;

            if (DateOfBirth > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
