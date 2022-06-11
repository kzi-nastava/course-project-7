using HIS.Core.MedicationModel.IngredientModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.MedicationModel
{
	public interface IMedicationRepository
	{
		public void Save();
		public IEnumerable<Medication> GetAll();
		public Medication Get(int id);
		public Medication Add(Medication obj);
		public void Remove(Medication obj);
		public int GetNextId();
		IEnumerable<Medication> GetAllThatUse(Ingredient ingredient);
	}
}
