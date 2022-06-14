using HIS.Core.PersonModel.UserAccountModel;
using System;
using System.Collections.Generic;

namespace HIS.CLI.View.UserCommand
{
	internal class ManagerCommandView : UserCommandView
	{
		public ManagerCommandView(UserAccount user, RoomView roomView, EquipmentView equipmentView, EquipmentRelocationView equipmentRelocationView, RenovationView renovationView, IngredientView ingredientView, MedicationView medicationView, PollSummaryView pollSummaryView) : base(user)
		{
			AddCommands(new Dictionary<string, Action>
			{
				{ "room-create",			() => roomView.CmdCreate() },
				{ "room-read",				() => roomView.CmdRead() },
				{ "room-update",			() => roomView.CmdUpdate() },
				{ "room-delete",			() => roomView.CmdDelete() },
				{ "equpiment-search",		() => equipmentView.CmdSearch() },
				{ "equpiment-view",			() => equipmentView.CmdFilter() },
				{ "equipment-relocate",		() => equipmentRelocationView.CmdPerform() },
				{ "renovation-schedule",	() => renovationView.CmdSchedule() },
				{ "ingredient-create",      () => ingredientView.CmdCreate() }, // TODO: Consistent naming for CRUD
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
