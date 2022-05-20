using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Util;

namespace HospitalIS.Backend.Controller
{
	internal static class IngredientController
	{
		public static List<Ingredient> GetIngredients()
		{
			return IS.Instance.Hospital.Ingredients.Where(ing => !ing.Deleted).ToList();
		}
	}
}
