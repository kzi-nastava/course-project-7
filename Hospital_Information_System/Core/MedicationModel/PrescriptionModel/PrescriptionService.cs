using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.MedicationModel.PrescriptionModel
{
	public class PrescriptionService : IPrescriptionService
	{
		private IPrescriptionRepository _repo;

		public PrescriptionService(IPrescriptionRepository repo)
		{
			_repo = repo;
		}

		public IEnumerable<Prescription> GetAll()
		{
			return _repo.GetAll();
		}
	}
}
