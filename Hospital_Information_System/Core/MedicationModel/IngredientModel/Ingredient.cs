using HIS.Core.Foundation;

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
	}
}
