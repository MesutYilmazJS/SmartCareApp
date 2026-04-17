using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCareApp.Data;
using SmartCareApp.Extensions;
using SmartCareApp.Models;
using SmartCareApp.Services;
using SmartCareApp.ViewModels;

namespace SmartCareApp.Controllers;

[Authorize(Roles = PortalRoles.Admin + "," + PortalRoles.Doctor)]
public class PatientsController(
    ApplicationDbContext dbContext,
    ITriageRiskService triageRiskService,
    IProfileImageCatalogService profileImageCatalogService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var patients = await dbContext.Patients
            .AsNoTracking()
            .OrderByDescending(patient => patient.RiskScore)
            .ThenBy(patient => patient.FullName)
            .ToListAsync();

        return View(patients);
    }

    [Authorize(Roles = PortalRoles.Doctor)]
    public IActionResult Create()
    {
        return View(new PatientCreateViewModel
        {
            Patient = new Patient
            {
                DateOfBirth = new DateOnly(1990, 1, 1)
            },
            ImageOptions = profileImageCatalogService.GetPatientImages()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = PortalRoles.Doctor)]
    public async Task<IActionResult> Create(PatientCreateViewModel model)
    {
        var patient = model.Patient;

        if (!string.IsNullOrWhiteSpace(patient.ProfileImageFileName) &&
            !profileImageCatalogService.PatientImageExists(patient.ProfileImageFileName))
        {
            ModelState.AddModelError("Patient.ProfileImageFileName", "Seçilen görsel bulunamadı.");
        }

        if (await dbContext.Patients.AnyAsync(item => item.NationalId == patient.NationalId))
        {
            ModelState.AddModelError("Patient.NationalId", "Bu T.C. kimlik numarası zaten kayıtlı.");
        }

        if (!ModelState.IsValid)
        {
            return View(new PatientCreateViewModel
            {
                Patient = patient,
                ImageOptions = profileImageCatalogService.GetPatientImages()
            });
        }

        var assessment = triageRiskService.Assess(patient);
        patient.RiskScore = assessment.RiskScore;
        patient.NoShowRiskScore = assessment.NoShowRiskScore;
        patient.CareStatus = PatientCareStatus.New;
        patient.AccessCode = await GenerateUniqueAccessCodeAsync();
        patient.CreatedByDoctorId = User.GetPortalUserId();

        if (string.IsNullOrWhiteSpace(patient.ProfileImageFileName))
        {
            patient.ProfileImageFileName = profileImageCatalogService.GetPatientImages().FirstOrDefault()?.FileName ?? string.Empty;
        }

        dbContext.Patients.Add(patient);
        await dbContext.SaveChangesAsync();

        dbContext.MedicalHistories.Add(new MedicalHistory
        {
            PatientId = patient.Id,
            RecordedAt = DateTimeOffset.UtcNow,
            Category = "Kayıt",
            Title = "Hasta kaydı oluşturuldu",
            Notes = $"{User.Identity?.Name} tarafından hasta kartı açıldı.",
            ClinicallyRelevant = true
        });

        await dbContext.SaveChangesAsync();

        TempData["StatusMessage"] = $"Hasta kaydı oluşturuldu. Giriş kodu: {patient.AccessCode}. Öncelik düzeyi: {assessment.PriorityLabel}.";

        return RedirectToAction(nameof(Details), new { id = patient.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var patient = await dbContext.Patients
            .Include(item => item.Appointments)
                .ThenInclude(appointment => appointment.Doctor)
            .Include(item => item.MedicalHistories)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (patient is null)
        {
            return NotFound();
        }

        var model = new PatientDetailsViewModel
        {
            Patient = patient,
            Assessment = triageRiskService.Assess(patient),
            UpcomingAppointments = patient.Appointments
                .Where(appointment => appointment.ScheduledAt >= DateTimeOffset.UtcNow.AddDays(-1))
                .OrderBy(appointment => appointment.ScheduledAt)
                .ToList(),
            ProfileImageUrl = profileImageCatalogService.GetPatientImageUrl(patient.ProfileImageFileName)
        };

        return View(model);
    }

    private async Task<string> GenerateUniqueAccessCodeAsync()
    {
        while (true)
        {
            var accessCode = $"H{RandomNumberGenerator.GetInt32(100000, 999999)}";

            var exists = await dbContext.Patients
                .AsNoTracking()
                .AnyAsync(patient => patient.AccessCode == accessCode);

            if (!exists)
            {
                return accessCode;
            }
        }
    }
}
