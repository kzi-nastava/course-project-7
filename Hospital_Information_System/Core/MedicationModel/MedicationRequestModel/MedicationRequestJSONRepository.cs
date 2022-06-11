using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HIS.Core.MedicationModel.MedicationRequestModel
{
	public class MedicationRequestJSONRepository : IMedicationRequestRepository
	{
		private IList<MedicationRequest> _requests;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public MedicationRequestJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			_requests = JsonConvert.DeserializeObject<List<MedicationRequest>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _requests.Count;
		}

		public IEnumerable<MedicationRequest> GetAll()
		{
			return _requests.Where(o => !o.Deleted);
		}

		public MedicationRequest Get(int id)
		{
			return _requests.FirstOrDefault(r => r.Id == id);
		}

		public MedicationRequest Add(MedicationRequest obj)
		{
			_requests.Add(obj);
			return obj;
		}

		public void Remove(MedicationRequest obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_requests, Formatting.Indented, _settings));
		}
	}
}
