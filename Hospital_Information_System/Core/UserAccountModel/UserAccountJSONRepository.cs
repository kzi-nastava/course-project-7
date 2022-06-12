using HIS.Core.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.UserAccountModel
{
	public class UserAccountJSONRepository : IUserAccountRepository
	{
		private readonly IList<UserAccount> _userAccounts;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public UserAccountJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			UserAccountJSONReferenceConverter.Repo = this;
			_userAccounts = JsonConvert.DeserializeObject<List<UserAccount>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _userAccounts.Count;
		}

		public IEnumerable<UserAccount> GetAll()
		{
			return _userAccounts.Where(o => !o.Deleted);
		}

		public UserAccount Get(int id)
		{
			return _userAccounts.FirstOrDefault(r => r.Id == id);
		}

		public UserAccount Add(UserAccount obj)
		{
			obj.Id = GetNextId();
			_userAccounts.Add(obj);
			return obj;
		}

		public void Remove(UserAccount obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_userAccounts, Formatting.Indented, _settings));
		}
	}
}
