using System.Collections.Generic;
using System.Linq;

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
			obj.Id = _repo.GetNextId();
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

		public IEnumerable<Ingredient> GetByName(string name)
		{
			return _repo.GetByName(name);
		}

		public void Copy(Ingredient src, Ingredient dest, IEnumerable<IngredientProperty> properties)
		{
			if (properties.Contains(IngredientProperty.NAME)) dest.Name = src.Name;
		}
	}
}
