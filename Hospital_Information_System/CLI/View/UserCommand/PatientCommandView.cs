using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.CLI.View.UserCommand
{
    internal class PatientCommandView : UserCommandView
    {
        public PatientCommandView(AppointmentView appointmentView, MedicalRecordView medicalRecordView, DoctorView doctorView, HospitalPollView hospitalPollView, AppointmentPollView appointmentPollView)
        {
            AddCommands(new Dictionary<string, Action>
            {
                { "app-create", () => appointmentView.CmdCreate() },
                { "app-read", () => appointmentView.CmdRead() },
                { "app-update", () => appointmentView.CmdUpdate() },
                { "app-delete", () => appointmentView.CmdDelete() },
                { "app-create-rec", () => appointmentView.CmdCreateRecommended() },
                { "anamnesis-search", () => medicalRecordView.CmdSearch() },
                { "account-notif", () => medicalRecordView.CmdChangeMinutesBeforeNotification() },
                { "doctor-search", () => doctorView.CmdSearch() },
                { "doctor-search-and-appoint", () => doctorView.CmdAppointFromSearch() },
                { "poll-hospital", () => hospitalPollView.CmdCreate() },
                { "poll-appointment", () => appointmentPollView.CmdCreate() },
            });
        }
    }
}
