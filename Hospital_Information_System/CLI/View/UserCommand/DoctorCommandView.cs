using System;
using System.Collections.Generic;

namespace HIS.CLI.View.UserCommand
{
    internal class DoctorCommandView : UserCommandView
    {

        public DoctorCommandView(AppointmentView appointmentView, DaysOffRequestView daysOffRequestView, MedicationView medicationView)
        {
            daysOffRequestView.NotifyDoctor();
            AddCommands(new Dictionary<string, Action>
            {
                { "appointment-create", () => appointmentView.CmdCreate() },
                { "appointment-read", () => appointmentView.CmdRead() },
                { "appointment-update", () => appointmentView.CmdUpdate() },
                { "appointment-delete", () => appointmentView.CmdDelete() },
                { "appointment-view-start", () => appointmentView.CmdViewAndStartAppointments() },
                { "medication-review", () => medicationView.CmdReviewMedicationRequests() },
                { "days-off-read", () => daysOffRequestView.CmdRead() },
                { "days-off-create", () => daysOffRequestView.CmdCreateDaysOffRequest() },
            });
        }
    }
}