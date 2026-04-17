using System.ComponentModel.DataAnnotations;

namespace SmartCareApp.ViewModels;

public class PatientLoginViewModel
{
    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [Display(Name = "T.C. kimlik no")]
    public string NationalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [Display(Name = "Giriş kodu")]
    public string AccessCode { get; set; } = string.Empty;
}
