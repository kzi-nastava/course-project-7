using HIS.Core.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.PersonModel
{
	public class PersonJSONRepository : IPersonRepository
	{
		private readonly IList<Person> _persons;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public PersonJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			PersonJSONReferenceConverter.Repo = this;
			_persons = JsonConvert.DeserializeObject<List<Person>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _persons.Count;
		}

		public IEnumerable<Person> GetAll()
		{
			return _persons.Where(o => !o.Deleted);
		}

		public Person Get(int id)
		{
			return _persons.FirstOrDefault(r => r.Id == id);
		}

		public Person Add(Person obj)
		{
			obj.Id = GetNextId();
			_persons.Add(obj);
			return obj;
		}

		public void Remove(Person obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_persons, Formatting.Indented, _settings));
		}
	}
}
