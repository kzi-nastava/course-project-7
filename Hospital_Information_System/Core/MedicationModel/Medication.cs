using HIS.Core.Foundation;
using HIS.Core.MedicationModel.IngredientModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.MedicationModel
{
	public enum MedicationProperty
	{
		NAME, INGREDIENTS
	}

	public class Medication : Entity
	{
		public string Name { get; set; }
		[JsonConverter(typeof(IngredientJSONIListConverter))]
		public IList<Ingredient> Ingredients { get; set; }

		public Medication()
		{

		}

		public override string ToString()
		{
			return $"Medication{{Name={Name}, Ingredients=[{Ingredients.Select(ing => ing.Name).Aggregate((s1, s2) => s1 + ", " + s2)}]}}";
		}
	}
}
