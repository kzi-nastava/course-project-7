using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
	internal static class IngredientModel
	{

		private static readonly string hintNameExists = "Ingredient with this name already exists";
		private static readonly string hintInputName = "Input name";
		private static readonly string hintSelectIngredient = "Select ingredient by number";
		private static readonly string hintSelectProperties = "Select properties, separated by whitespace. Input a blank line to finish selection";
		private static readonly string warningDependantMedicine = "The following medicine will be removed. Do you wish to proceed?";

		public static void Create(string inputCancelString) 
		{
			var ingredient = InputIngredient(IngredientController.ingredientPropertiesAll, inputCancelString);
			IS.Instance.IngredientRepo.Add(ingredient);
		}

		public static void Update(string inputCancelString)
		{
			Console.WriteLine(hintSelectIngredient);
			var ingredient = EasyInput<Ingredient>.Select(IngredientController.GetIngredients(), inputCancelString);
			Console.WriteLine(hintSelectProperties);
			var properties = EasyInput<IngredientController.IngredientProperty>.SelectMultiple(IngredientController.ingredientPropertiesAll, inputCancelString).ToList();
			var ingredientNew = InputIngredient(properties, inputCancelString);
			Copy(ingredientNew, ingredient, properties);
		}

		public static void Read(string inputCancelString)
		{
			foreach (var ing in IngredientController.GetIngredients())
			{
				Console.WriteLine(ing);
			}
		}

		public static void Delete(string inputCancelString)
		{
			var ingredientsToRemove = EasyInput<Ingredient>.SelectMultiple(IngredientController.GetIngredients(), inputCancelString);
			var dependentMedicine = MedicationController.GetMedications().Where(med => med.Ingredients.Intersect(ingredientsToRemove).Count() != 0);

			if (dependentMedicine.Count() != 0)
			{
				foreach (var med in dependentMedicine)
				{
					Console.WriteLine(med);
				}
				Console.WriteLine(warningDependantMedicine);
				if (!EasyInput<bool>.YesNo(inputCancelString))
				{
					return;
				}
			}

			foreach (var ingredient in ingredientsToRemove)
			{
				IS.Instance.IngredientRepo.Remove(ingredient);
			}
		}

		private static Ingredient InputIngredient(List<IngredientController.IngredientProperty> whichProperties, string inputCancelString)
		{
			Ingredient ingredient = new Ingredient();

			if (whichProperties.Contains(IngredientController.IngredientProperty.NAME)) {
				Console.WriteLine(hintInputName);
				ingredient.Name = InputIngredientName(inputCancelString);
			}

			return ingredient;
		}

		private static string InputIngredientName(string inputCancelString) {
			return EasyInput<string>.Get(
				new List<Func<string, bool>>() {
					s => IngredientController.GetIngredients().Count(ing => ing.Name.ToLower().Equals(s.ToLower())) == 0
				},
				new[] {
					hintNameExists
				},
				inputCancelString
			);
		}

		private static void Copy(Ingredient source, Ingredient desitnation, List<IngredientController.IngredientProperty> whichProperties)
		{
			if (whichProperties.Contains(IngredientController.IngredientProperty.NAME))
				desitnation.Name = source.Name;
		}
	}
}
