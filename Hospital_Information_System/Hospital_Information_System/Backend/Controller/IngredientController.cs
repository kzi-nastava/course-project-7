using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Util;

namespace HospitalIS.Backend.Controller
{
	internal static class IngredientController
	{
		internal enum IngredientProperty {
			NAME
		}

		internal static readonly List<IngredientProperty> ingredientPropertiesAll = Enum.GetValues(typeof(IngredientProperty)).Cast<IngredientProperty>().ToList();

		public static List<Ingredient> GetIngredients()
		{
			return IS.Instance.Hospital.Ingredients.Where(ing => !ing.Deleted).ToList();
		}
	}
}
