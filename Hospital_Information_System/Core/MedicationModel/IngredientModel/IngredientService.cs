using System.Collections.Generic;

namespace HIS.Core.MedicationModel.IngredientModel
{
	public class IngredientService : IIngredientService
	{
		private IIngredientRepository _repo;

		public IngredientService(IIngredientRepository repo)
		{
			_repo = repo;
		}

		public Ingredient Add(Ingredient obj)
		{
			return _repo.Add(obj);
		}

		public IEnumerable<Ingredient> GetAll()
		{
			return _repo.GetAll();
		}

		public void Remove(Ingredient obj)
		{
			_repo.Remove(obj);
		}
	}
}
