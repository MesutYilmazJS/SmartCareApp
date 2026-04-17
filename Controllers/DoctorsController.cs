using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCareApp.Data;
using SmartCareApp.Models;

namespace SmartCareApp.Controllers;

[Authorize(Roles = PortalRoles.Admin + "," + PortalRoles.Doctor)]
public class DoctorsController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var doctors = await dbContext.Doctors
            .AsNoTracking()
            .Include(doctor => doctor.Appointments)
            .OrderBy(doctor => doctor.FullName)
            .ToListAsync();

        return View(doctors);
    }
}
