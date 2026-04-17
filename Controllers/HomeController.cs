using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartCareApp.Models;

namespace SmartCareApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole(PortalRoles.Admin))
            {
                return RedirectToAction("Index", "Admin");
            }

            if (User.IsInRole(PortalRoles.Doctor))
            {
                return RedirectToAction("Index", "DoctorPortal");
            }

            if (User.IsInRole(PortalRoles.Patient))
            {
                return RedirectToAction("Index", "PatientPortal");
            }
        }

        ViewData["HideChrome"] = true;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
