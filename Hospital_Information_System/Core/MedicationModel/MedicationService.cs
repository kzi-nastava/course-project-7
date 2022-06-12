using HIS.Core.MedicationModel.IngredientModel;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.MedicationModel
{
	public class MedicationService : IMedicationService
	{
		private IMedicationRepository _repo;

		public MedicationService(IMedicationRepository repo)
		{
			_repo = repo;
		}

		public Medication Add(Medication obj)
		{
			return _repo.Add(obj);
		}

		public Medication Get(int id)
		{
			return _repo.Get(id);
		}

		public IEnumerable<Medication> GetAll()
		{
			return _repo.GetAll();
		}

		public void Remove(Medication obj)
		{
			_repo.Remove(obj);
		}

		public IEnumerable<Medication> GetAllThatUse(Ingredient ingredient)
		{
			return _repo.GetAllThatUse(ingredient);
		}

		public IEnumerable<Medication> GetByName(string name)
		{
			return _repo.GetByName(name);
		}

		public void Copy(Medication src, Medication dest, IEnumerable<MedicationProperty> properties)
		{
			if (properties.Contains(MedicationProperty.NAME)) dest.Name = src.Name;
			if (properties.Contains(MedicationProperty.INGREDIENTS)) dest.Ingredients = new List<Ingredient>(src.Ingredients);
		}
	}
}
