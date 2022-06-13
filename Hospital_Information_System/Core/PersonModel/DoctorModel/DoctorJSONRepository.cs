using HIS.Core.PersonModel.DoctorModel.DoctorComparers;
using HIS.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.PersonModel.DoctorModel
{
	public class DoctorJSONRepository : IDoctorRepository
	{
		private readonly IList<Doctor> _doctors;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public DoctorJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			DoctorJSONReferenceConverter.Repo = this;
			_doctors = JsonConvert.DeserializeObject<List<Doctor>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _doctors.Count;
		}

		public IEnumerable<Doctor> GetAll()
		{
			return _doctors.Where(o => !o.Deleted);
		}

		public Doctor Get(int id)
		{
			return _doctors.FirstOrDefault(r => r.Id == id);
		}

		public Doctor Add(Doctor obj)
		{
			obj.Id = GetNextId();
			_doctors.Add(obj);
			return obj;
		}

		public void Remove(Doctor obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_doctors, Formatting.Indented, _settings));
		}

        public IEnumerable<Doctor> MatchByString(string query, DoctorComparer comparer, Func<Doctor, string> toStr)
		{
			var matches = GetAll().ToList().FindAll(d => toStr(d).Contains(query.Trim(), StringComparison.OrdinalIgnoreCase));
			matches.Sort(comparer);
			return matches;
		}

        public IEnumerable<Doctor> MatchByFirstName(string query, DoctorComparer comparer)
		{
			return MatchByString(query, comparer, d => d.Person.FirstName);
		}

        public IEnumerable<Doctor> MatchByLastName(string query, DoctorComparer comparer)
		{
			return MatchByString(query, comparer, d => d.Person.LastName);
		}

        public IEnumerable<Doctor> MatchBySpecialty(string query, DoctorComparer comparer)
		{
			return MatchByString(query, comparer, d => d.Specialty.ToString());
		}
    }
}
