using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.MedicationModel.IngredientModel
{
	public interface IIngredientRepository
	{
		public void Save();
		public IEnumerable<Ingredient> GetAll();
		public Ingredient Get(int id);
		public Ingredient Add(Ingredient obj);
		public void Remove(Ingredient obj);
		public int GetNextId();
		public IEnumerable<Ingredient> GetByName(string name);
	}
}
