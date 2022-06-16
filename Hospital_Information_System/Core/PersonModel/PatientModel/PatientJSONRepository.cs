using HIS.Core.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.PersonModel.PatientModel
{
    public class PatientJSONRepository : IPatientRepository
	{
		private readonly IList<Patient> _patients;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public PatientJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			PatientJSONReferenceConverter.Repo = this;
			_patients = JsonConvert.DeserializeObject<List<Patient>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _patients.Count;
		}

		public IEnumerable<Patient> GetAll()
		{
			return _patients.Where(o => !o.Deleted);
		}

		public Patient Get(int id)
		{
			return _patients.FirstOrDefault(r => r.Id == id);
		}

		public Patient Add(Patient obj)
		{
			obj.Id = GetNextId();
			_patients.Add(obj);
			return obj;
		}

		public void Remove(Patient obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_patients, Formatting.Indented, _settings));
		}
	}
}
