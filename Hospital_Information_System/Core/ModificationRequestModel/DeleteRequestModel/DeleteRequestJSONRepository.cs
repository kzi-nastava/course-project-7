using HIS.Core.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.ModificationRequestModel.DeleteRequestModel
{
	public class DeleteRequestJSONRepository : IDeleteRequestRepository
	{
		private readonly IList<DeleteRequest> _deleteRequests;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public DeleteRequestJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			DeleteRequestJSONReferenceConverter.Repo = this;
			_deleteRequests = JsonConvert.DeserializeObject<List<DeleteRequest>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _deleteRequests.Count;
		}

		public IEnumerable<DeleteRequest> GetAll()
		{
			return _deleteRequests.Where(o => !o.Deleted);
		}

		public DeleteRequest Get(int id)
		{
			return _deleteRequests.FirstOrDefault(r => r.Id == id);
		}

		public DeleteRequest Add(DeleteRequest obj)
		{
			obj.Id = GetNextId();
			_deleteRequests.Add(obj);
			return obj;
		}

		public void Remove(DeleteRequest obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_deleteRequests, Formatting.Indented, _settings));
		}
	}
}
