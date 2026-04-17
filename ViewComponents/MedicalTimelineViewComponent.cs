using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCareApp.Data;
using SmartCareApp.Extensions;
using SmartCareApp.ViewModels;

namespace SmartCareApp.ViewComponents;

public class MedicalTimelineViewComponent(ApplicationDbContext dbContext) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(int patientId)
    {
        var clinicalEvents = await dbContext.MedicalHistories
            .AsNoTracking()
            .Where(history => history.PatientId == patientId)
            .Select(history => new MedicalTimelineItemViewModel
            {
                OccurredAt = history.RecordedAt,
                Category = history.Category,
                Title = history.Title,
                Detail = history.Notes,
                Tone = history.ClinicallyRelevant ? "timeline-item-strong" : "timeline-item-muted"
            })
            .ToListAsync();

        var appointmentEvents = await dbContext.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Doctor)
            .Where(appointment => appointment.PatientId == patientId)
            .Select(appointment => new MedicalTimelineItemViewModel
            {
                OccurredAt = appointment.ScheduledAt,
                Category = "Randevu",
                Title = $"{appointment.VisitType} - {appointment.Doctor.FullName}",
                Detail = $"{appointment.Status.ToDisplayText()}: {appointment.Reason}",
                Tone = appointment.Status == Models.AppointmentStatus.Completed
                    ? "timeline-item-strong"
                    : "timeline-item-neutral"
            })
            .ToListAsync();

        var timeline = clinicalEvents
            .Concat(appointmentEvents)
            .OrderByDescending(item => item.OccurredAt)
            .ToList();

        return View(timeline);
    }
}
