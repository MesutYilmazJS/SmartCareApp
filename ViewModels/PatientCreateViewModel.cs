using SmartCareApp.Models;

namespace SmartCareApp.ViewModels;

public class PatientCreateViewModel
{
    public Patient Patient { get; init; } = new()
    {
        DateOfBirth = new DateOnly(1990, 1, 1)
    };

    public IReadOnlyList<ProfileImageOptionViewModel> ImageOptions { get; init; } = Array.Empty<ProfileImageOptionViewModel>();
}
