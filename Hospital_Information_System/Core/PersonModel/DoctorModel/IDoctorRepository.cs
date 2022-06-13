using System;
using System.Collections.Generic;

namespace HIS.Core.DoctorModel
{
	public interface IDoctorRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<Doctor> GetAll();
		public Doctor Get(int id);
		public Doctor Add(Doctor obj);
		public void Remove(Doctor obj);
	}
}
