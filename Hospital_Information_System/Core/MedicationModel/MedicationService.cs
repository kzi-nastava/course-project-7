using HIS.Core.MedicationModel.IngredientModel;
using System.Collections.Generic;

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
			obj.Id = _repo.GetNextId();
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
	}
}
