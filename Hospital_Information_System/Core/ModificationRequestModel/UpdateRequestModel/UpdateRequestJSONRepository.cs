using HIS.Core.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.ModificationRequestModel.UpdateRequestModel
{
	public class UpdateRequestJSONRepository : IUpdateRequestRepository
	{
		private readonly IList<UpdateRequest> _updateRequests;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public UpdateRequestJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			UpdateRequestJSONReferenceConverter.Repo = this;
			_updateRequests = JsonConvert.DeserializeObject<List<UpdateRequest>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _updateRequests.Count;
		}

		public IEnumerable<UpdateRequest> GetAll()
		{
			return _updateRequests.Where(o => !o.Deleted);
		}

		public UpdateRequest Get(int id)
		{
			return _updateRequests.FirstOrDefault(r => r.Id == id);
		}

		public UpdateRequest Add(UpdateRequest obj)
		{
			obj.Id = GetNextId();
			_updateRequests.Add(obj);
			return obj;
		}

		public void Remove(UpdateRequest obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_updateRequests, Formatting.Indented, _settings));
		}
	}
}
