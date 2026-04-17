using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Http;
using SmartCareApp.ViewModels;

namespace SmartCareApp.Services;

public class ProfileImageCatalogService(IWebHostEnvironment environment) : IProfileImageCatalogService
{
    private const string RequestRoot = "/media";
    private readonly string? _imagesRootPath = FindImagesRoot(environment.ContentRootPath);

    public IReadOnlyList<ProfileImageOptionViewModel> GetDoctorImages() => GetImages("doktorlar");

    public IReadOnlyList<ProfileImageOptionViewModel> GetPatientImages() => GetImages("hastalar");

    public string GetDoctorImageUrl(string? fileName) => GetImageUrl("doktorlar", fileName);

    public string GetPatientImageUrl(string? fileName) => GetImageUrl("hastalar", fileName);

    public bool DoctorImageExists(string? fileName) => ImageExists("doktorlar", fileName);

    public bool PatientImageExists(string? fileName) => ImageExists("hastalar", fileName);

    public string? GetImagesRootPath() => _imagesRootPath;

    public async Task<string> SaveImageAsync(IFormFile file, string folderName)
    {
        if (_imagesRootPath is null) throw new InvalidOperationException("Images root path not found.");

        var folderPath = Path.Combine(_imagesRootPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }

    private IReadOnlyList<ProfileImageOptionViewModel> GetImages(string folderName)
    {
        var folderPath = GetFolderPath(folderName);

        if (folderPath is null)
        {
            return Array.Empty<ProfileImageOptionViewModel>();
        }

        return Directory.EnumerateFiles(folderPath)
            .Where(path => !Path.GetFileName(path).StartsWith(".", StringComparison.Ordinal))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Select(path => new ProfileImageOptionViewModel
            {
                FileName = Path.GetFileName(path),
                Url = $"{RequestRoot}/{folderName}/{Uri.EscapeDataString(Path.GetFileName(path))}"
            })
            .ToList();
    }

    private string GetImageUrl(string folderName, string? fileName)
    {
        if (!ImageExists(folderName, fileName))
        {
            var firstImage = GetImages(folderName).FirstOrDefault();
            return firstImage?.Url ?? string.Empty;
        }

        return $"{RequestRoot}/{folderName}/{Uri.EscapeDataString(fileName!)}";
    }

    private bool ImageExists(string folderName, string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        var folderPath = GetFolderPath(folderName);

        return folderPath is not null &&
               File.Exists(Path.Combine(folderPath, fileName));
    }

    private string? GetFolderPath(string folderName)
    {
        if (_imagesRootPath is null)
        {
            return null;
        }

        var fullPath = Path.Combine(_imagesRootPath, folderName);
        return Directory.Exists(fullPath) ? fullPath : null;
    }

    private static string? FindImagesRoot(string contentRootPath)
    {
        return Directory.EnumerateDirectories(contentRootPath)
            .FirstOrDefault(path => Normalize(Path.GetFileName(path)).Contains("gorseller", StringComparison.Ordinal));
    }

    private static string Normalize(string value)
    {
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
