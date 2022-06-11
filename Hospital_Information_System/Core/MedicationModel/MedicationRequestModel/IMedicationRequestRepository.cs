using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.MedicationModel.MedicationRequestModel
{
	public interface IMedicationRequestRepository
	{
		public void Save();
		public IEnumerable<MedicationRequest> GetAll();
		public MedicationRequest Get(int id);
		public MedicationRequest Add(MedicationRequest obj);
		public void Remove(MedicationRequest obj);
		public int GetNextId();
	}
}
