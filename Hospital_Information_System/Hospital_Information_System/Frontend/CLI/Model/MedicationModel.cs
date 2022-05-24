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

		internal static void CreateNewMedicine(string inputCancelString)
		{
			Medication medication = InputMedication(MedicationController.medicationPropertiesAll, inputCancelString);

			MedicationRequest medicationRequest = new MedicationRequest(medication);
			IS.Instance.MedicationRequestRepo.Add(medicationRequest);
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
				medication.Ingredients = InputMedicationIngredients(inputCancelString);
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
	}
}