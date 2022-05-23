using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
	internal static class IngredientModel
	{
		private enum IngredientProperty {
			NAME
		}

		private static readonly List<IngredientProperty> ingredientPropertiesAll = Enum.GetValues(typeof(IngredientProperty)).Cast<IngredientProperty>().ToList();

		private static readonly string hintNameExists = "Ingredient with this name already exists";

		private static readonly string hintInputName = "Input name";

		public static void Create(string inputCancelString) 
		{
			var ingredient = InputIngredient(ingredientPropertiesAll, inputCancelString);
			IS.Instance.IngredientRepo.Add(ingredient);
		}

		private static Ingredient InputIngredient(List<IngredientProperty> whichProperties, string inputCancelString)
		{
			Ingredient ingredient = new Ingredient();

			if (whichProperties.Contains(IngredientProperty.NAME)) {
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
	}
}
