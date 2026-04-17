using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCareApp.Data;
using SmartCareApp.Extensions;
using SmartCareApp.Models;
using SmartCareApp.Services;
using SmartCareApp.ViewModels;

namespace SmartCareApp.Controllers;

[Authorize(Roles = PortalRoles.Doctor)]
[Route("doktor")]
public class DoctorPortalController(
    ApplicationDbContext dbContext,
    IProfileImageCatalogService profileImageCatalogService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var doctorId = User.GetPortalUserId();

        if (doctorId is null)
        {
            return Forbid();
        }

        var doctor = await dbContext.Doctors
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == doctorId.Value);

        if (doctor is null)
        {
            return Forbid();
        }

        var patients = await dbContext.Patients
            .AsNoTracking()
            .Include(patient => patient.Appointments)
            .Where(patient => patient.CreatedByDoctorId == doctorId.Value)
            .OrderByDescending(patient => patient.RiskScore)
            .ThenBy(patient => patient.FullName)
            .ToListAsync();

        var appointmentsDb = await dbContext.Appointments
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId.Value)
            .ToListAsync();

        var appointments = appointmentsDb
            .Where(a => a.ScheduledAt >= DateTimeOffset.UtcNow.AddDays(-1))
            .OrderBy(a => a.ScheduledAt)
            .ToList();

        var model = new DoctorPortalViewModel
        {
            Doctor = doctor,
            ProfileImageUrl = profileImageCatalogService.GetDoctorImageUrl(doctor.ProfileImageFileName),
            Patients = patients
                .Select(patient => new DoctorPatientRowViewModel
                {
                    Id = patient.Id,
                    FullName = patient.FullName,
                    Age = patient.Age,
                    NationalId = patient.NationalId,
                    AccessCode = patient.AccessCode,
                    PrimarySymptom = patient.PrimarySymptom,
                    ProfileImageUrl = profileImageCatalogService.GetPatientImageUrl(patient.ProfileImageFileName),
                    RiskScore = patient.RiskScore,
                    NoShowRiskScore = patient.NoShowRiskScore,
                    CareStatus = patient.CareStatus,
                    LastVisitText = patient.Appointments
                        .OrderByDescending(appointment => appointment.ScheduledAt)
                        .Select(appointment => appointment.ScheduledAt.ToLocalTime().ToString("dd MMM HH:mm"))
                        .FirstOrDefault() ?? "Randevu yok"
                })
                .ToList(),
            UpcomingAppointments = appointments
        };

        return View(model);
    }

    [HttpPost("profil-gorseli")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfileImage(string fileName)
    {
        var doctorId = User.GetPortalUserId();

        if (doctorId is null || !profileImageCatalogService.DoctorImageExists(fileName))
        {
            return RedirectToAction(nameof(Index));
        }

        var doctor = await dbContext.Doctors.FirstOrDefaultAsync(item => item.Id == doctorId.Value);

        if (doctor is null)
        {
            return RedirectToAction(nameof(Index));
        }

        doctor.ProfileImageFileName = fileName;
        await dbContext.SaveChangesAsync();

        TempData["StatusMessage"] = "Profil görseli güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("durum-guncelle")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePatientStatus(int patientId, PatientCareStatus status)
    {
        var doctorId = User.GetPortalUserId();

        if (doctorId is null)
        {
            return Forbid();
        }

        var doctor = await dbContext.Doctors.FirstOrDefaultAsync(item => item.Id == doctorId.Value);
        var patient = await dbContext.Patients.FirstOrDefaultAsync(item =>
            item.Id == patientId &&
            item.CreatedByDoctorId == doctorId.Value);

        if (doctor is null || patient is null)
        {
            return RedirectToAction(nameof(Index));
        }

        patient.CareStatus = status;

        dbContext.MedicalHistories.Add(new MedicalHistory
        {
            PatientId = patient.Id,
            RecordedAt = DateTimeOffset.UtcNow,
            Category = "Durum",
            Title = $"Durum güncellendi: {status.ToDisplayText()}",
            Notes = $"{doctor.FullName}, hastanın durumunu {status.ToDisplayText().ToLowerInvariant()} olarak işaretledi.",
            ClinicallyRelevant = status is PatientCareStatus.InTreatment or PatientCareStatus.UnderReview
        });

        await dbContext.SaveChangesAsync();

        TempData["StatusMessage"] = $"{patient.FullName} için durum {status.ToDisplayText()} olarak güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("iptal-onayla")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveCancellation(int appointmentId)
    {
        var doctorId = User.GetPortalUserId();
        if (doctorId == null) return Forbid();

        var appointment = await dbContext.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorId == doctorId.Value);

        if (appointment == null) return NotFound();

        if (appointment.Status == AppointmentStatus.CancellationRequested)
        {
            appointment.Status = AppointmentStatus.Cancelled;
            
            dbContext.MedicalHistories.Add(new MedicalHistory
            {
                PatientId = appointment.PatientId,
                RecordedAt = DateTimeOffset.UtcNow,
                Category = "Randevu",
                Title = "Randevu İptal Edildi",
                Notes = $"Doktor {User.Identity?.Name}, hastanın iptal talebi üzerine randevuyu iptal etti."
            });

            await dbContext.SaveChangesAsync();
            TempData["StatusMessage"] = "Randevu iptali onaylandı.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("iptal-reddet")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectCancellation(int appointmentId)
    {
        var doctorId = User.GetPortalUserId();
        if (doctorId == null) return Forbid();

        var appointment = await dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorId == doctorId.Value);

        if (appointment == null) return NotFound();

        if (appointment.Status == AppointmentStatus.CancellationRequested)
        {
            appointment.Status = AppointmentStatus.Scheduled;
            await dbContext.SaveChangesAsync();
            TempData["StatusMessage"] = "İptal talebi reddedildi, randevu planlandığı gibi devam ediyor.";
        }

        return RedirectToAction(nameof(Index));
    }
}
