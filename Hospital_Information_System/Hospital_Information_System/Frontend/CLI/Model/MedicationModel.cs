using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
	internal abstract class MedicationModel
	{
		private const string errMedicationExists = "Medication with that name already exists";
		private const string hintInputName = "Input medication name";
		private const string hintInputIngredients = "Select ingredients for this medication by number, separated by whitespace. Input an empty line to finish";
		private const string hintSelectRequests = "Select requests you want to review, separated by whitespace. Input an empty line to finish";
		private const string hintInputRequestState = "Select state for the selected request";
		private const string hintInputComment = "Input a comment for your decision: ";
		private const string errNoIngredients = "There are no ingredients registered in the system. Medication will be empty";
		private const string errNoMedicationsForRevision = "There are no medications sent for revision";
		private const string errMedicationForReview = "There is no medication to be reviewed";

		internal static void CreateNewMedicine(string inputCancelString)
		{
			Medication medication = InputMedication(MedicationController.medicationPropertiesAll, inputCancelString);

			MedicationRequest medicationRequest = new MedicationRequest(medication);
			IS.Instance.MedicationRequestRepo.Add(medicationRequest);
		}

		internal static void ReviseMedicine(string inputCancelString)
		{
			MedicationRequest medRequest = null;
			try {
				medRequest = EasyInput<MedicationRequest>.Select(MedicationRequestController.GetRequestsForRevision(), r => r.Medication.Name, inputCancelString);

				// TODO @magley: Remove if check in production.  
				if (medRequest.Reviews.Count() > 0) {
					Console.WriteLine($"@{medRequest.Reviews.Last().Timestamp}, {medRequest.Reviews.Last().Reviewer.ToString()}:");
					Console.WriteLine(medRequest.Reviews.Last().Message);
				}
			}
			catch (NothingToSelectException) {
				Console.WriteLine(errNoMedicationsForRevision);
				return;
			}

			var whichProperties = EasyInput<MedicationController.MedicationProperty>.SelectMultiple(MedicationController.medicationPropertiesAll, inputCancelString).ToList();
			var modifiedMedication = InputMedication(whichProperties, inputCancelString);
			MedicationController.Copy(modifiedMedication, medRequest.Medication, whichProperties);
			medRequest.State = MedicationRequestState.SENT;
		}

		private static Medication InputMedication(List<MedicationController.MedicationProperty> whichProperties, string inputCancelString) 
		{
			Medication medication = new Medication();

			if (whichProperties.Contains(MedicationController.MedicationProperty.NAME))
			{
				Console.WriteLine(hintInputName);
				medication.Name = InputMedicationName(inputCancelString);
			}
			if (whichProperties.Contains(MedicationController.MedicationProperty.INGREDIENTS))
			{
				Console.WriteLine(hintInputIngredients);

				try {
					medication.Ingredients = InputMedicationIngredients(inputCancelString);
				}
				catch (NothingToSelectException) {
					Console.WriteLine(errNoIngredients);
					medication.Ingredients = new List<Ingredient>();
				}
			}

			return medication;
		}

		private static string InputMedicationName(string inputCancelString) 
		{
			return EasyInput<string>.Get(
				new List<Func<string, bool>>() {
					s => !MedicationController.GetMedications().Any(med => med.Name.ToLower().Equals(s.ToLower()))
				},
				new[] {
					errMedicationExists
				},
				inputCancelString
			);
		}

		private static List<Ingredient> InputMedicationIngredients(string inputCancelString)
		{
			return EasyInput<Ingredient>.SelectMultiple(IngredientController.GetIngredients(), inputCancelString).ToList();
		}
		
		public static void ReviewMedicationRequests(UserAccount user, string inputCancelString)
		{
			try
			{
				Doctor reviewer = DoctorController.GetDoctorFromPerson(user.Person);
				List<MedicationRequest> allRequests = MedicationRequestController.GetMedicationRequests(reviewer);
				Console.WriteLine(hintSelectRequests);
				List<MedicationRequest> selectedRequests =
					EasyInput<MedicationRequest>.SelectMultiple(allRequests, inputCancelString).ToList();
				foreach (var request in selectedRequests)
				{
					Console.WriteLine(request);
					Review(request, reviewer, inputCancelString);
				}
			}
			catch (NothingToSelectException)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(errMedicationForReview);
				Console.ForegroundColor = ConsoleColor.Gray;
			}

		}
		
		private static void Review(MedicationRequest request, Doctor reviewer, string inputCancelString)
		{
			List<MedicationRequestState> requestStates = MedicationRequestController.GetRequestStatesForDoctor();
			Console.WriteLine(hintInputRequestState);
			var chosenState = EasyInput<MedicationRequestState>.Select(requestStates, inputCancelString);
			Console.WriteLine(hintInputComment);
			string message = Console.ReadLine();
			MedicationRequestReview review = new MedicationRequestReview(reviewer, message, chosenState);
			
			
			if (chosenState == MedicationRequestState.APPROVED)
			{
				MedicationRequestController.Accept(ref request, ref review);
			}
			
			else if (chosenState == MedicationRequestState.REJECTED)
			{
				MedicationRequestController.Reject(ref request, ref review);
			}
			//(chosenState == MedicationRequestState.RETURNED)
			else
			{
				MedicationRequestController.SendForRevision(ref request, ref review);
			}
			
		}
	}
}