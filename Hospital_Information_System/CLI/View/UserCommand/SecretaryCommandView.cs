using System;
using System.Collections.Generic;

namespace HIS.CLI.View.UserCommand
{
    internal class SecretaryCommandView : UserCommandView
    {
        public SecretaryCommandView(UserAccountView userAccountView, AppointmentView appointmentView, RequestView requestView, EquipmentView equipmentView, EquipmentRelocationView equipmentRelocationView, DaysOffRequestView daysOffRequestView)
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
                {"handle-referrals", () => requestView.CmdHandleRefferals()}, 
                {"create-urgent-appointment", () => appointmentView.CmdCreateUrgent()}, 
                {"request-new-equipment", () => equipmentView.CmdRequestNew()},
                {"move-dynamic-equipment", () => equipmentRelocationView.CmdMoveDynamicEquipment()},
                {"/", () => daysOffRequestView.CmdHandle()} //handle-days-off-requests
            });
        }
    }
}