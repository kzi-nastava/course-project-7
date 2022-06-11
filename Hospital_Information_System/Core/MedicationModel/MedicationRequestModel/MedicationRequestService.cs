using HIS.Core.MedicationModel.IngredientModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.MedicationModel.MedicationRequestModel
{
	public class MedicationRequestService : IMedicationRequestService
	{
		private IMedicationRequestRepository _repo;

		public MedicationRequestService(IMedicationRequestRepository repo)
		{
			_repo = repo;
		}

		public MedicationRequest Add(MedicationRequest medicationRequest)
		{
			medicationRequest.Id = _repo.GetNextId();
			return _repo.Add(medicationRequest);
		}

		public IEnumerable<MedicationRequest> GetAllThatUse(Ingredient ing)
		{
			return _repo.GetAllThatUse(ing);
		}

		public void Remove(MedicationRequest req)
		{
			_repo.Remove(req);
		}

		public IEnumerable<MedicationRequest> GetAllReturnedForRevision()
		{
			return _repo.GetAllReturnedForRevision();
		}
	}
}
