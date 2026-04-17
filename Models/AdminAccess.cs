using System.ComponentModel.DataAnnotations;

namespace SmartCareApp.Models;

public class AdminAccess
{
    public int Id { get; set; }

    [Required(ErrorMessage = "{0} alanı zorunludur.")]
    [StringLength(20, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
    [Display(Name = "Yönetici kodu")]
    public string AccessCode { get; set; } = string.Empty;
}
