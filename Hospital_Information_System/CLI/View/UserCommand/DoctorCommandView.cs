using System;
using System.Collections.Generic;

namespace HIS.CLI.View.UserCommand
{
    internal class DoctorCommandView : UserCommandView
    {
        public DoctorCommandView(AppointmentView appointmentView, DaysOffRequestView daysOffRequestView)
        {
            AddCommands(new Dictionary<string, Action>
            {
                { "app-create", () => appointmentView.CmdCreate() },
                { "app-read", () => appointmentView.CmdRead() },
                { "app-update", () => appointmentView.CmdUpdate() },
                { "app-delete", () => appointmentView.CmdDelete() },
                { "app-view-start", () => appointmentView.CmdViewAndStartAppointments() },
                { "dor-read", () => daysOffRequestView.CmdRead() },
                { "dor-create", () => daysOffRequestView.CmdCreateDaysOffRequest() },
            });
        }
    }
}