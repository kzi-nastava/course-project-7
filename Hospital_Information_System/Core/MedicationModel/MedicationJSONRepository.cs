using HIS.Core.MedicationModel.IngredientModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.MedicationModel
{
	public class MedicationJSONRepository : IMedicationRepository
	{
		private IList<Medication> _medication;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public MedicationJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			_medication = JsonConvert.DeserializeObject<List<Medication>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _medication.Count;
		}

		public IEnumerable<Medication> GetAll()
		{
			return _medication.Where(o => !o.Deleted);
		}

		public Medication Get(int id)
		{
			return _medication.FirstOrDefault(r => r.Id == id);
		}

		public Medication Add(Medication obj)
		{
			obj.Id = GetNextId();
			_medication.Add(obj);
			return obj;
		}

		public void Remove(Medication obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_medication, Formatting.Indented, _settings));
		}

		public IEnumerable<Medication> GetAllThatUse(Ingredient ingredient)
		{
			return GetAll().Where(med => med.Ingredients.Contains(ingredient));
		}

		public IEnumerable<Medication> GetByName(string name)
		{
			return GetAll().Where(med => med.Name == name);
		}
	}
}
