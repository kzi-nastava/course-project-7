using System;
using System.Collections.Generic;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel
{
	public interface IMedicalRecordRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<MedicalRecord> GetAll();
		public MedicalRecord Get(int id);
		public MedicalRecord Add(MedicalRecord obj);
		public void Remove(MedicalRecord obj);
	}
}
