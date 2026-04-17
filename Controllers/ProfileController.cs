using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCareApp.Data;
using SmartCareApp.Extensions;
using SmartCareApp.Models;
using SmartCareApp.Services;

namespace SmartCareApp.Controllers;

[Authorize]
public class ProfileController(
    ApplicationDbContext dbContext,
    IProfileImageCatalogService profileImageCatalogService) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadPatientImage(int? id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        int targetId;
        if (id.HasValue)
        {
            // Doctor or Admin editing a patient
            if (!User.IsInRole(PortalRoles.Doctor) && !User.IsInRole(PortalRoles.Admin))
            {
                return Forbid();
            }
            targetId = id.Value;
        }
        else
        {
            // Patient editing their own profile
            var patientId = User.GetPortalUserId();
            if (patientId == null || !User.IsInRole(PortalRoles.Patient))
            {
                return Forbid();
            }
            targetId = patientId.Value;
        }

        var patient = await dbContext.Patients.FirstOrDefaultAsync(p => p.Id == targetId);
        if (patient == null)
        {
            return NotFound();
        }

        var fileName = await profileImageCatalogService.SaveImageAsync(file, "hastalar");
        patient.ProfileImageFileName = fileName;
        await dbContext.SaveChangesAsync();

        TempData["StatusMessage"] = "Profil görseli güncellendi.";
        
        if (User.IsInRole(PortalRoles.Patient) && !id.HasValue)
        {
            return RedirectToAction("Index", "PatientPortal");
        }
        
        return Redirect(Request.Headers["Referer"].ToString() ?? "/");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadDoctorImage(int? id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        int targetId;
        if (id.HasValue)
        {
            // Admin editing a doctor
            if (!User.IsInRole(PortalRoles.Admin))
            {
                return Forbid();
            }
            targetId = id.Value;
        }
        else
        {
            // Doctor editing their own profile
            var doctorId = User.GetPortalUserId();
            if (doctorId == null || !User.IsInRole(PortalRoles.Doctor))
            {
                return Forbid();
            }
            targetId = doctorId.Value;
        }

        var doctor = await dbContext.Doctors.FirstOrDefaultAsync(d => d.Id == targetId);
        if (doctor == null)
        {
            return NotFound();
        }

        var fileName = await profileImageCatalogService.SaveImageAsync(file, "doktorlar");
        doctor.ProfileImageFileName = fileName;
        await dbContext.SaveChangesAsync();

        TempData["StatusMessage"] = "Profil görseli güncellendi.";

        if (User.IsInRole(PortalRoles.Doctor) && !id.HasValue)
        {
            return RedirectToAction("Index", "DoctorPortal");
        }

        return Redirect(Request.Headers["Referer"].ToString() ?? "/");
    }
}
