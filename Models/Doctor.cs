using System.ComponentModel.DataAnnotations;

namespace SmartCareApp.Models;

public class Doctor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [StringLength(120, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Ad soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [StringLength(80, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Branş")]
    public string Specialty { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [StringLength(40, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Oda")]
    public string RoomNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [StringLength(40, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Kullanıcı adı")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [StringLength(80, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [StringLength(120, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Profil görseli")]
    public string ProfileImageFileName { get; set; } = string.Empty;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Patient> CreatedPatients { get; set; } = new List<Patient>();
}
