using HIS.Core.MedicationModel;
using HIS.Core.MedicationModel.IngredientModel;
using HIS.Core.MedicationModel.MedicationRequestModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.CLI.View
{
	internal class MedicationView : View
	{
		private readonly IMedicationService _service;
		private readonly IIngredientService _ingredientService;
		private readonly IMedicationRequestService _medicationRequestService;
		private IEnumerable<MedicationProperty> _properties;

		private static readonly string hintInputName = "Enter name";
		private static readonly string hintSelectIngredients = "Select ingredients";
		private static readonly string errNameExists = "Name already exists";
		private static readonly string hintNothingToSelect = "Nothing to select. Stop";

		public MedicationView(IMedicationService service, IIngredientService ingredientService, IMedicationRequestService medicationRequestService, UserAccount user) : base(user)
		{
			_service = service;
			_ingredientService = ingredientService;
			_medicationRequestService = medicationRequestService;
			_properties = Utility.GetEnumValues<MedicationProperty>();
		}

		internal void CmdCreateAndSendForReview()
		{
			var medication = Input(_properties);
			_medicationRequestService.Add(new MedicationRequest(medication));
		}

		internal void CmdUpdateRequest()
		{
			try
			{
				var request = EasyInput<MedicationRequest>.Select(_medicationRequestService.GetAllReturnedForRevision(), request => request.Medication.Name, _cancel);
				Print(request.ToString());

				var propertiesToUpdate = EasyInput<MedicationProperty>.SelectMultiple(_properties.ToList(), _cancel);
				Medication changed = Input(propertiesToUpdate);

				_service.Copy(changed, request.Medication, propertiesToUpdate);
				request.State = MedicationRequestState.SENT;
			}
			catch (NothingToSelectException)
			{
				Hint(hintNothingToSelect);
			}
		}

		private Medication Input(IEnumerable<MedicationProperty> whichProperties)
		{
			Medication m = new Medication();

			if (whichProperties.Contains(MedicationProperty.NAME))
			{
				Hint(hintInputName);
				m.Name = InputName();
			}
			if (whichProperties.Contains(MedicationProperty.INGREDIENTS))
			{
				Hint(hintSelectIngredients);
				m.Ingredients = InputIngredients().ToList();
			}

			return m;
		}

		private string InputName()
		{
			return EasyInput<string>.Get(
				new List<Func<string, bool>>() { s => _service.GetByName(s).Count() == 0 },
				new[] { errNameExists },
				_cancel
			);
		}

		private IList<Ingredient> InputIngredients()
		{
			return EasyInput<Ingredient>.SelectMultiple(_ingredientService.GetAll().ToList(), _cancel);
		}
	}
}
