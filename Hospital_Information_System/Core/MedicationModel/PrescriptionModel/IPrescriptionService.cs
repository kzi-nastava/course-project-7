using System.Collections.Generic;

namespace HIS.Core.MedicationModel.PrescriptionModel
{
	public interface IPrescriptionService
	{
		public IEnumerable<Prescription> GetAll();
	}
}
