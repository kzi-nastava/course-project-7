using HIS.Core.EquipmentModel;
using HIS.Core.EquipmentModel.EquipmentRelocationModel;
using HIS.Core.MedicationModel;
using HIS.Core.MedicationModel.IngredientModel;
using HIS.Core.MedicationModel.MedicationRequestModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.RoomModel;
using HIS.Core.RoomModel.RenovationModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.CLI.View.UserCommand
{
	internal class ManagerCommandView : UserCommandView
	{
		RoomView roomView;
		EquipmentView equipmentView;
		EquipmentRelocationView equipmentRelocationView;
		RenovationView renovationView;
		IngredientView ingredientView;
		MedicationView medicationView;
		PollSummaryView pollSummaryView;

		public ManagerCommandView(UserAccount user, RoomView roomView, EquipmentView equipmentView, EquipmentRelocationView equipmentRelocationView, RenovationView renovationView, IngredientView ingredientView, MedicationView medicationView, PollSummaryView pollSummaryView) : base(user)
		{
			this.roomView = roomView;
			this.equipmentView = equipmentView;
			this.equipmentRelocationView = equipmentRelocationView;
			this.renovationView = renovationView;
			this.ingredientView = ingredientView;
			this.medicationView = medicationView;
			this.pollSummaryView = pollSummaryView;

			AddCommands(new Dictionary<string, Action>
			{
				{ "room-create",			() => roomView.CmdRoomCreate() },
				{ "room-read",				() => roomView.CmdRoomView() },
				{ "room-update",			() => roomView.CmdRoomUpdate() },
				{ "room-delete",			() => roomView.CmdRoomDelete() },
				{ "equpiment-search",		() => equipmentView.CmdSearch() },
				{ "equpiment-view",			() => equipmentView.CmdFilter() },
				{ "equipment-relocate",		() => equipmentRelocationView.CmdPerform() },
				{ "renovation-schedule",	() => renovationView.CmdSchedule() },
				{ "ingredient-create",      () => ingredientView.CmdAdd() }, // TODO: Consistent naming for CRUD
				{ "ingredient-read",        () => ingredientView.CmdRead() },
				{ "ingredient-update",      () => ingredientView.CmdUpdate() },
				{ "ingredient-delete",      () => ingredientView.CmdDelete() },
				{ "medication-create",      () => medicationView.CmdCreateAndSendForReview() },
				{ "medication-revise",      () => medicationView.CmdUpdateRequest() },
				{ "poll-hospital-average",  () => pollSummaryView.CmdPrintHospitalAverageRatings() },
				{ "poll-hospital-one",      () => pollSummaryView.CmdPrintSingleHospitalPoll() },
				{ "poll-hospital-comments", () => pollSummaryView.CmdPrintAllHospitalComments() },
				{ "poll-doctor-top3",       () => pollSummaryView.CmdPrintDoctorTop3() },
				{ "poll-doctor-summary",	() => pollSummaryView.CmdPrintDoctorRatings() },
			});
		}
	}
}
