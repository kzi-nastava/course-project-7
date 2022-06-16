using System;
using System.Collections.Generic;

namespace HIS.CLI.View.UserCommand
{
    internal class SecretaryCommandView : UserCommandView
    {
        public SecretaryCommandView(UserAccountView userAccountView, AppointmentView appointmentView, MedicalRecordView medicalRecordView, RequestView requestView, EquipmentView equipmentView)
        {
            AddCommands(new Dictionary<string, Action>
            {
                {"patient-account-create", () => userAccountView.CmdCreate()},
                {"patient-account-view", () => userAccountView.CmdRead()},
                {"patient-account-update", () => userAccountView.CmdUpdate()},
                {"patient-account-delete", () => userAccountView.CmdDelete()},
                {"block-patient-account", () => userAccountView.CmdBlock()},
                {"unblock-patient-account", () => userAccountView.CmdUnblock()},
                {"view-patient-requests", () => requestView.CmdRead()},
                {"handle-patient-requests", () => requestView.CmdHandle()},
                {"handle-referrals", () => throw new NotImplementedException()}, 
                {"create-urgent-appointment", () => appointmentView.CmdCreateUrgent()}, 
                {"request-new-equipment", () => equipmentView.CmdRequestNew()},
                {"move-dynamic-equipment", () => throw new NotImplementedException()}
            });
        }
    }
}