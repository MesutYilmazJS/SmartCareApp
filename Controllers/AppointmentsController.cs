using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCareApp.Data;
using SmartCareApp.Models;

namespace SmartCareApp.Controllers;

[Authorize(Roles = PortalRoles.Admin + "," + PortalRoles.Doctor)]
public class AppointmentsController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var appointments = (await dbContext.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
            .ToListAsync())
            .OrderBy(appointment => appointment.ScheduledAt)
            .ToList();

        return View(appointments);
    }
}
