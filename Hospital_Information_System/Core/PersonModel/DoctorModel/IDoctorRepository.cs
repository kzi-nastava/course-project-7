using HIS.Core.PersonModel.DoctorModel.DoctorComparers;
using System;
using System.Collections.Generic;

namespace HIS.Core.PersonModel.DoctorModel
{
	public interface IDoctorRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<Doctor> GetAll();
		public Doctor Get(int id);
		public Doctor Add(Doctor obj);
		public void Remove(Doctor obj);
		public IEnumerable<Doctor> MatchByString(string query, DoctorComparer comparer, Func<Doctor, string> toStr);
		public IEnumerable<Doctor> MatchByFirstName(string query, DoctorComparer comparer);
		public IEnumerable<Doctor> MatchByLastName(string query, DoctorComparer comparer);
		public IEnumerable<Doctor> MatchBySpecialty(string query, DoctorComparer comparer);
	}
}
