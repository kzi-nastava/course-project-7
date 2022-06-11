using HIS.Core.Foundation;
using HIS.Core.MedicationModel.IngredientModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.MedicationModel
{
	public class Medication : Entity
	{
		public string Name { get; set; }
		[JsonConverter(typeof(IngredientJSONIListConverter))]
		public IList<Ingredient> Ingredients { get; private set; }

		public Medication()
		{

		}

		public Medication(string name, IList<Ingredient> ingredients)
		{
			Name = name;
			Ingredients = ingredients;
		}

		public override string ToString()
		{
			return $"Medication{{Name={Name}, Ingredients=[{Ingredients.Select(ing => ing.ToString() + ", ")}]}}";
		}
	}
}
