using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.MedicationModel.PrescriptionModel
{
	public class PrescriptionJSONRepository : IPrescriptionRepository
	{
		private IList<Prescription> _prescriptions;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public PrescriptionJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			PrescriptionJSONIListConverter.Repo = this;
			_prescriptions = JsonConvert.DeserializeObject<List<Prescription>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _prescriptions.Count;
		}

		public IEnumerable<Prescription> GetAll()
		{
			return _prescriptions.Where(o => !o.Deleted);
		}

		public Prescription Get(int id)
		{
			return _prescriptions.FirstOrDefault(r => r.Id == id);
		}

		public Prescription Add(Prescription obj)
		{
			obj.Id = GetNextId();
			_prescriptions.Add(obj);
			return obj;
		}

		public void Remove(Prescription obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_prescriptions, Formatting.Indented, _settings));
		}
	}
}
