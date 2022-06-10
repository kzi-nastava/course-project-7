using System.Collections.Generic;

namespace HIS.Core.MedicationModel.IngredientModel
{
	public interface IIngredientService
	{
		public Ingredient Add(Ingredient obj);
		public void Remove(Ingredient obj);
		public IEnumerable<Ingredient> GetAll();
	}
}
