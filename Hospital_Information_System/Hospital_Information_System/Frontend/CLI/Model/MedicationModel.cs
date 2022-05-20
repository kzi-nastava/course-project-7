using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
	internal abstract class MedicationModel
	{
		private enum MedicationProperty
		{
			NAME, INGREDIENTS
		}
		private static readonly List<MedicationProperty> medicationPropertiesAll = Enum.GetValues(typeof(MedicationProperty)).Cast<MedicationProperty>().ToList();
		private static readonly string errMedicationExists = "Medication with that name already exists";
		private static readonly string hintInputName = "Input medication name";
		private static readonly string hintInputIngredients = "Select ingredients for this medication by number, separated by whitespace. Input an empty line to finish";

		public static void CreateNewMedicine(string inputCancelString)
		{
			Medication medication = InputMedication(medicationPropertiesAll, inputCancelString);
			IS.Instance.MedicationRepo.Add(medication);
		}

		private static Medication InputMedication(List<MedicationProperty> whichProperties, string inputCancelString) 
		{
			Medication medication = new Medication();

			if (whichProperties.Contains(MedicationProperty.NAME))
			{
				Console.WriteLine(hintInputName);
				medication.Name = InputMedicationName(inputCancelString);
			}
			if (whichProperties.Contains(MedicationProperty.INGREDIENTS))
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
					s => MedicationController.GetMedications().Count(med => med.Name.ToLower().Equals(s.ToLower())) == 0
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