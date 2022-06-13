using HIS.Core.PersonModel.PatientModel;
using HIS.Core.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.PollModel.HospitalPollModel
{
	public class HospitalPollJSONRepository : IHospitalPollRepository
	{
		private readonly IList<HospitalPoll> _hospitalPolls;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public HospitalPollJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			_hospitalPolls = JsonConvert.DeserializeObject<List<HospitalPoll>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _hospitalPolls.Count;
		}

		public IEnumerable<HospitalPoll> GetAll()
		{
			return _hospitalPolls.Where(o => !o.Deleted);
		}

		public HospitalPoll Get(int id)
		{
			return _hospitalPolls.FirstOrDefault(r => r.Id == id);
		}

		public HospitalPoll Add(HospitalPoll obj)
		{
			obj.Id = GetNextId();
			_hospitalPolls.Add(obj);
			return obj;
		}

		public void Remove(HospitalPoll obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_hospitalPolls, Formatting.Indented, _settings));
		}

        public IEnumerable<HospitalPoll> GetAll(Patient pollee)
		{
			return GetAll().Where(hp => !hp.Deleted && hp.Pollee == pollee);
		}
    }
}
