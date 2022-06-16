using System.Collections.Generic;

namespace HIS.Core.MedicationModel.PrescriptionModel
{
	public interface IPrescriptionService
	{
		public IEnumerable<Prescription> GetAll();
		public void Save();
		public Prescription Get(int id);
		public Prescription Add(Prescription obj);
		public void Remove(Prescription obj);
		public int GetNextId();
		public List<Prescription.UsageTypes> GetAllMedicationUsages();
	}
}
