using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCareApp.Data;
using SmartCareApp.Extensions;
using SmartCareApp.Models;
using SmartCareApp.Services;
using SmartCareApp.ViewModels;

namespace SmartCareApp.Controllers;

[Route("admin")]
public class AdminController(
    ApplicationDbContext dbContext,
    IProfileImageCatalogService profileImageCatalogService) : Controller
{
    [AllowAnonymous]
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true && !User.IsInRole(PortalRoles.Admin))
        {
            return Forbid();
        }

        if (!User.IsInRole(PortalRoles.Admin))
        {
            ViewData["HideChrome"] = true;
            return View("Login", new AdminLoginViewModel());
        }

        return View(await BuildDashboardAsync());
    }

    [AllowAnonymous]
    [HttpPost("giris")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["HideChrome"] = true;
            return View("Login", model);
        }

        var isValid = await dbContext.AdminAccessCodes
            .AsNoTracking()
            .AnyAsync(item => item.AccessCode == model.AccessCode.Trim());

        if (!isValid)
        {
            ModelState.AddModelError(nameof(model.AccessCode), "Yönetici kodu doğrulanamadı.");
            ViewData["HideChrome"] = true;
            return View("Login", model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "0"),
            new(ClaimTypes.Name, "Yönetici"),
            new(ClaimTypes.Role, PortalRoles.Admin)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = PortalRoles.Admin)]
    private async Task<DashboardViewModel> BuildDashboardAsync()
    {
        var now = DateTimeOffset.UtcNow;
        var today = now.Date;

        var patients = await dbContext.Patients
            .AsNoTracking()
            .OrderByDescending(patient => patient.RiskScore)
            .ToListAsync();

        var doctors = await dbContext.Doctors
            .AsNoTracking()
            .Include(doctor => doctor.Appointments)
            .OrderBy(doctor => doctor.FullName)
            .ToListAsync();

        var upcomingAppointments = (await dbContext.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
            .ToListAsync())
            .Where(appointment => appointment.ScheduledAt >= now.AddDays(-2))
            .OrderBy(appointment => appointment.ScheduledAt)
            .Take(8)
            .ToList();

        return new DashboardViewModel
        {
            ActivePatientCount = patients.Count,
            UrgentQueueCount = patients.Count(patient => patient.RiskScore >= 70),
            NoShowAlertCount = patients.Count(patient => patient.NoShowRiskScore >= 55),
            TodayAppointmentCount = upcomingAppointments.Count(appointment => appointment.ScheduledAt.Date == today),
            PriorityQueue = patients
                .Take(6)
                .Select(patient => new DashboardPatientItemViewModel
                {
                    PatientId = patient.Id,
                    FullName = patient.FullName,
                    Age = patient.Age,
                    PrimarySymptom = patient.PrimarySymptom,
                    RiskScore = patient.RiskScore,
                    NoShowRiskScore = patient.NoShowRiskScore,
                    ChronicConditions = patient.ChronicConditions,
                    ProfileImageUrl = profileImageCatalogService.GetPatientImageUrl(patient.ProfileImageFileName)
                })
                .ToList(),
            Insights = new List<DashboardInsightViewModel>
            {
                new()
                {
                    Title = "Öncelik skoru",
                    Description = "Belirti, yaş ve ek hastalık bilgileri bir araya getirilerek bakım önceliği tek ekranda öne çıkarılır.",
                    Accent = "accent-critical"
                },
                new()
                {
                    Title = "Katılım riski",
                    Description = "Geçmiş randevu davranışı ve portal hareketi düşük olan hastalar planlamada ayrı izlenir.",
                    Accent = "accent-warning"
                },
                new()
                {
                    Title = "Hasta zaman akışı",
                    Description = "Geçmiş muayene, not ve randevular hasta detay ekranında tek bir akış içinde görülür.",
                    Accent = "accent-info"
                }
            },
            DoctorLoads = doctors
                .Select(doctor => new DoctorLoadViewModel
                {
                    FullName = doctor.FullName,
                    Specialty = doctor.Specialty,
                    RoomNumber = doctor.RoomNumber,
                    AppointmentCount = doctor.Appointments.Count(appointment => appointment.ScheduledAt.Date == today),
                    ProfileImageUrl = profileImageCatalogService.GetDoctorImageUrl(doctor.ProfileImageFileName)
                })
                .OrderByDescending(doctor => doctor.AppointmentCount)
                .ThenBy(doctor => doctor.FullName)
                .ToList(),
            UpcomingAppointments = upcomingAppointments
                .Select(appointment => new DashboardAppointmentItemViewModel
                {
                    PatientName = appointment.Patient.FullName,
                    DoctorName = appointment.Doctor.FullName,
                    ScheduledAt = appointment.ScheduledAt,
                    Status = appointment.Status.ToDisplayText(),
                    NoShowRiskScore = appointment.PredictedNoShowRisk,
                    PatientProfileImageUrl = profileImageCatalogService.GetPatientImageUrl(appointment.Patient.ProfileImageFileName)
                })
                .ToList()
        };
    }
}
