using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.MedicationModel.IngredientModel
{
	public class IngredientJSONRepository : IIngredientRepository
	{
		private IList<Ingredient> _ingredients;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public IngredientJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			IngredientJSONIListConverter.Repo = this;
			_ingredients = JsonConvert.DeserializeObject<List<Ingredient>>(File.ReadAllText(fname), _settings);
		}

		// todo @magley : Add() should do id-assignment, not the service

		public int GetNextId()
		{
			return _ingredients.Count;
		}

		public IEnumerable<Ingredient> GetAll()
		{
			return _ingredients.Where(o => !o.Deleted);
		}

		public Ingredient Get(int id)
		{
			return _ingredients.FirstOrDefault(r => r.Id == id);
		}

		public Ingredient Add(Ingredient obj)
		{
			_ingredients.Add(obj);
			return obj;
		}

		public void Remove(Ingredient obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_ingredients, Formatting.Indented, _settings));
		}

		public IEnumerable<Ingredient> GetByName(string name)
		{
			return GetAll().Where(ing => ing.Name == name);
		}
	}
}
