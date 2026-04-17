using SmartCareApp.ViewModels;

namespace SmartCareApp.Services;

public interface IProfileImageCatalogService
{
    IReadOnlyList<ProfileImageOptionViewModel> GetDoctorImages();
    IReadOnlyList<ProfileImageOptionViewModel> GetPatientImages();
    string GetDoctorImageUrl(string? fileName);
    string GetPatientImageUrl(string? fileName);
    bool DoctorImageExists(string? fileName);
    bool PatientImageExists(string? fileName);
    string? GetImagesRootPath();
    Task<string> SaveImageAsync(IFormFile file, string folderName);
}
