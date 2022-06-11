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
	}
}
