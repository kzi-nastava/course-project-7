using HIS.Core.Foundation;
using System.Collections.Generic;

namespace HIS.Core.MedicationModel.IngredientModel
{
	public enum IngredientProperty { NAME }

	public class Ingredient : Entity
	{
		public string Name { get; set; }

		public Ingredient()
		{
		}

		public Ingredient(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return $"Ingredient{{Id={Id}, Name={Name}}}";
		}

		public static string IngredientsToString(List<Ingredient> entry)
		{
			string result = "";
			for (int i = 0; i <= entry.Count - 1; i++)
			{
				result += entry[i].Name;
				if (i < entry.Count - 1)
				{
					result += ", ";
				}
			}

			return result;
		}
	}
}
