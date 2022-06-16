using System;
using System.Collections.Generic;

namespace HIS.CLI.View.UserCommand
{
    internal class DoctorCommandView : UserCommandView
    {
        public DoctorCommandView(AppointmentView appointmentView)
        {
            AddCommands(new Dictionary<string, Action>
            {
                { "app-create", () => appointmentView.CmdCreate() },
                { "app-read", () => appointmentView.CmdRead() },
                { "app-update", () => appointmentView.CmdUpdate() },
                { "app-delete", () => appointmentView.CmdDelete() },
                { "app-view-start", () => appointmentView.CmdViewAndStartAppointments() },
            });
        }
    }
}