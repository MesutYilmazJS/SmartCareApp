using System.ComponentModel.DataAnnotations;

namespace SmartCareApp.ViewModels;

public class DoctorLoginViewModel
{
    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [Display(Name = "Kullanıcı adı")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;
}
