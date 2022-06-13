using HIS.Core.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel
{
	public class MedicalRecordJSONRepository : IMedicalRecordRepository
	{
		private readonly IList<MedicalRecord> _medicalRecords;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public MedicalRecordJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			MedicalRecordJSONReferenceConverter.Repo = this;
			_medicalRecords = JsonConvert.DeserializeObject<List<MedicalRecord>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _medicalRecords.Count;
		}

		public IEnumerable<MedicalRecord> GetAll()
		{
			return _medicalRecords.Where(o => !o.Deleted);
		}

		public MedicalRecord Get(int id)
		{
			return _medicalRecords.FirstOrDefault(r => r.Id == id);
		}

		public MedicalRecord Add(MedicalRecord obj)
		{
			obj.Id = GetNextId();
			_medicalRecords.Add(obj);
			return obj;
		}

		public void Remove(MedicalRecord obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_medicalRecords, Formatting.Indented, _settings));
		}
	}
}
