using HIS.Core.MedicationModel.IngredientModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HIS.Core.PersonModel.DoctorModel;

namespace HIS.Core.MedicationModel.MedicationRequestModel
{
	public class MedicationRequestService : IMedicationRequestService
	{
		private IMedicationRequestRepository _repo;
		private IMedicationService _medicationService;

		public MedicationRequestService(IMedicationRequestRepository repo, IMedicationService medicationService)
		{
			_repo = repo;
			_medicationService = medicationService;
		}

		public MedicationRequest Add(MedicationRequest medicationRequest)
		{
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

		public List<MedicationRequest> Get(Doctor doctor)
		{
			return _repo.Get(doctor);
		}
		
		public IEnumerable<MedicationRequestState> GetAllRequestStates()
		{
			return _repo.GetAllRequestStates();
		}
		
		public List<MedicationRequestState> GetRequestStatesForDoctor()
		{
			List<MedicationRequestState> states = GetAllRequestStates().ToList();
			states.Remove(MedicationRequestState.SENT);
			return states;
		}

		public void Accept(ref MedicationRequest request, ref MedicationReview review)
		{
			request.State = MedicationRequestState.APPROVED;
			request.Reviews.Add(review);
			var newMedication = request.Medication;
			_medicationService.Add(newMedication);
		}

		public void Reject(ref MedicationRequest request, ref MedicationReview review)
		{
			request.State = MedicationRequestState.REJECTED;
			request.Reviews.Add(review);
			_repo.Remove(request);
		}

		public void SendForRevision(ref MedicationRequest request, ref MedicationReview review)
		{
			request.State = MedicationRequestState.RETURNED;
			request.Reviews.Add(review);
		}

		public IEnumerable<MedicationRequest> GetAllSent()
		{
			return _repo.GetAllSent();
		}
	}
}
