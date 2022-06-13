using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.MedicationModel.PrescriptionModel
{
	public interface IPrescriptionRepository
	{
		public void Save();
		public IEnumerable<Prescription> GetAll();
		public Prescription Get(int id);
		public Prescription Add(Prescription obj);
		public void Remove(Prescription obj);
		public int GetNextId();
	}
}
