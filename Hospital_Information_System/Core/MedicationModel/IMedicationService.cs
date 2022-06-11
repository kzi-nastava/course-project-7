using HIS.Core.MedicationModel.IngredientModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.MedicationModel
{
	public interface IMedicationService
	{
		public Medication Add(Medication obj);
		public Medication Get(int id);
		public IEnumerable<Medication> GetAll();
		public void Remove(Medication obj);
		public IEnumerable<Medication> GetAllThatUse(Ingredient ingredient);
	}
}
