using System;
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

		public void Save()
		{
			_repo.Save();
		}

		public Prescription Get(int id)
		{
			return _repo.Get(id);
		}

		public Prescription Add(Prescription obj)
		{
			return _repo.Add(obj);
		}

		public void Remove(Prescription obj)
		{
			_repo.Remove(obj);
		}

		public int GetNextId()
		{
			return _repo.GetNextId();
		}
		
		public List<Prescription.UsageTypes> GetAllMedicationUsages()
		{
			return Enum.GetValues(typeof(Prescription.UsageTypes)).Cast<Prescription.UsageTypes>().ToList();
		}
	}
}
