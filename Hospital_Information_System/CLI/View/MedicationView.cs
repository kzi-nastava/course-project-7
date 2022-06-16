using HIS.Core.MedicationModel;
using HIS.Core.MedicationModel.IngredientModel;
using HIS.Core.MedicationModel.MedicationRequestModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.PersonModel.DoctorModel;

namespace HIS.CLI.View
{
	internal class MedicationView : AbstractView
	{
		private readonly IMedicationService _service;
		private readonly IIngredientService _ingredientService;
		private readonly IMedicationRequestService _medicationRequestService;
		private IEnumerable<MedicationProperty> _properties;
		private IDoctorService _doctorService;

		private static readonly string hintInputName = "Enter name";
		private static readonly string hintSelectIngredients = "Select ingredients";
		private static readonly string errNameExists = "Name already exists";
		private static readonly string hintNothingToSelect = "Nothing to select. Stop";
		private const string hintSelectRequests = "Select requests you want to review, separated by whitespace. Input an empty line to finish";
		private const string hintInputRequestState = "Select state for the selected request";
		private const string hintInputComment = "Input a comment for your decision: ";
		private const string errMedicationForReview = "There is no medication to be reviewed";

		public MedicationView(IMedicationService service, IIngredientService ingredientService, IMedicationRequestService medicationRequestService, IDoctorService doctorService)
		{
			_service = service;
			_ingredientService = ingredientService;
			_medicationRequestService = medicationRequestService;
			_properties = Utility.GetEnumValues<MedicationProperty>();
			_doctorService = doctorService;
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
		
		internal void CmdReviewMedicationRequests()
		{
			try
			{
				Doctor reviewer = _doctorService.GetDoctorFromPerson(User.Person);
				List<MedicationRequest> allRequests = _medicationRequestService.Get(reviewer);
				Hint(hintSelectRequests);
				List<MedicationRequest> selectedRequests =
					EasyInput<MedicationRequest>.SelectMultiple(allRequests, _cancel).ToList();
				foreach (var request in selectedRequests)
				{
					Print(request.ToString());
					Review(request, reviewer);
				}
			}
			catch (NothingToSelectException)
			{
				Error(errMedicationForReview);
			}

		}
		
		private void Review(MedicationRequest request, Doctor reviewer)
		{
			List<MedicationRequestState> requestStates = _medicationRequestService.GetRequestStatesForDoctor();
			Hint(hintInputRequestState);
			var chosenState = EasyInput<MedicationRequestState>.Select(requestStates, _cancel);
			Hint(hintInputComment);
			string message = Console.ReadLine();
			MedicationReview review = new MedicationReview(message, DateTime.Now, chosenState, reviewer);
			
			
			if (chosenState == MedicationRequestState.APPROVED)
			{
				_medicationRequestService.Accept(ref request, ref review);
			}
			
			else if (chosenState == MedicationRequestState.REJECTED)
			{
				_medicationRequestService.Reject(ref request, ref review);
			}
			//(chosenState == MedicationRequestState.RETURNED)
			else
			{
				_medicationRequestService.SendForRevision(ref request, ref review);
			}
			
		}
	}
}
