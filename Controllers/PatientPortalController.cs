using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCareApp.Data;
using SmartCareApp.Extensions;
using SmartCareApp.Models;
using SmartCareApp.Services;
using SmartCareApp.ViewModels;

namespace SmartCareApp.Controllers;

[Authorize(Roles = PortalRoles.Patient)]
[Route("hasta")]
public class PatientPortalController(
    ApplicationDbContext dbContext,
    ITriageRiskService triageRiskService,
    IProfileImageCatalogService profileImageCatalogService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var patientId = User.GetPortalUserId();

        if (patientId is null)
        {
            return Forbid();
        }

        var patient = await dbContext.Patients
            .Include(item => item.Appointments)
                .ThenInclude(appointment => appointment.Doctor)
            .Include(item => item.MedicalHistories)
            .FirstOrDefaultAsync(item => item.Id == patientId.Value);

        if (patient is null)
        {
            return Forbid();
        }

        var model = new PatientPortalViewModel
        {
            Patient = patient,
            Assessment = triageRiskService.Assess(patient),
            ProfileImageUrl = profileImageCatalogService.GetPatientImageUrl(patient.ProfileImageFileName),
            UpcomingAppointments = patient.Appointments
                .Where(appointment => appointment.ScheduledAt >= DateTimeOffset.UtcNow.AddDays(-1))
                .OrderBy(appointment => appointment.ScheduledAt)
                .ToList()
        };

        return View(model);
    }

    [HttpPost("profil-gorseli")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfileImage(string fileName)
    {
        var patientId = User.GetPortalUserId();

        if (patientId is null || !profileImageCatalogService.PatientImageExists(fileName))
        {
            return RedirectToAction(nameof(Index));
        }

        var patient = await dbContext.Patients.FirstOrDefaultAsync(item => item.Id == patientId.Value);

        if (patient is null)
        {
            return RedirectToAction(nameof(Index));
        }

        patient.ProfileImageFileName = fileName;
        await dbContext.SaveChangesAsync();

        TempData["StatusMessage"] = "Profil görseli güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("randevu-iptal-talebi")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestCancellation(int appointmentId)
    {
        var patientId = User.GetPortalUserId();

        if (patientId is null)
        {
            return Forbid();
        }

        var appointment = await dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.PatientId == patientId.Value);

        if (appointment == null)
        {
            return NotFound();
        }

        if (appointment.Status == AppointmentStatus.Scheduled)
        {
            appointment.Status = AppointmentStatus.CancellationRequested;
            await dbContext.SaveChangesAsync();
            TempData["StatusMessage"] = "İptal talebi oluşturuldu. Doktor onayı bekleniyor.";
        }

        return RedirectToAction(nameof(Index));
    }
}
