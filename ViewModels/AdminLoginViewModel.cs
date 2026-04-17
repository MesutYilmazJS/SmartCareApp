using System.ComponentModel.DataAnnotations;

namespace SmartCareApp.ViewModels;

public class AdminLoginViewModel
{
    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [Display(Name = "Yönetici kodu")]
    public string AccessCode { get; set; } = string.Empty;
}
