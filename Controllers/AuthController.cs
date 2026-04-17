using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCareApp.Data;
using SmartCareApp.Models;
using SmartCareApp.ViewModels;

namespace SmartCareApp.Controllers;

[AllowAnonymous]
public class AuthController(ApplicationDbContext dbContext) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoctorLogin(DoctorLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["DoctorLoginError"] = "Doktor girişi için kullanıcı adı ve şifre gerekiyor.";
            return RedirectToAction("Index", "Home");
        }

        var doctor = await dbContext.Doctors
            .AsNoTracking()
            .FirstOrDefaultAsync(item =>
                item.Username == model.Username.Trim() &&
                item.Password == model.Password);

        if (doctor is null)
        {
            TempData["DoctorLoginError"] = "Doktor bilgileri doğrulanamadı.";
            return RedirectToAction("Index", "Home");
        }

        await SignInAsync(doctor.Id, doctor.FullName, PortalRoles.Doctor);

        return RedirectToAction("Index", "DoctorPortal");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PatientLogin(PatientLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["PatientLoginError"] = "Hasta girişi için T.C. kimlik no ve giriş kodu gerekiyor.";
            return RedirectToAction("Index", "Home");
        }

        var patient = await dbContext.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(item =>
                item.NationalId == model.NationalId.Trim() &&
                item.AccessCode == model.AccessCode.Trim().ToUpperInvariant());

        if (patient is null)
        {
            TempData["PatientLoginError"] = "Hasta giriş bilgileri doğrulanamadı.";
            return RedirectToAction("Index", "Home");
        }

        await SignInAsync(patient.Id, patient.FullName, PortalRoles.Patient);

        return RedirectToAction("Index", "PatientPortal");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInAsync(int id, string fullName, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, id.ToString()),
            new(ClaimTypes.Name, fullName),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true
            });
    }
}
