using System;
using System.Collections.Generic;

namespace HIS.Core.PatientModel
{
    public interface IPatientRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<Patient> GetAll();
		public Patient Get(int id);
		public Patient Add(Patient obj);
		public void Remove(Patient obj);
	}
}
