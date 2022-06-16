using HIS.Core.MedicationModel.IngredientModel;
using System;
using System.Collections.Generic;
using System.Text;
using HIS.Core.PersonModel.DoctorModel;

namespace HIS.Core.MedicationModel.MedicationRequestModel
{
	public interface IMedicationRequestService
	{
		public MedicationRequest Add(MedicationRequest medicationRequest);
		public IEnumerable<MedicationRequest> GetAllThatUse(Ingredient ing);
		public void Remove(MedicationRequest req);
		public IEnumerable<MedicationRequest> GetAllReturnedForRevision();
		public List<MedicationRequest> Get(Doctor doctor);
		public IEnumerable<MedicationRequestState> GetAllRequestStates();
		public List<MedicationRequestState> GetRequestStatesForDoctor();
		public void Accept(ref MedicationRequest request, ref MedicationReview review);
		public void Reject(ref MedicationRequest request, ref MedicationReview review);
		public void SendForRevision(ref MedicationRequest request, ref MedicationReview review);
		public IEnumerable<MedicationRequest> GetAllSent();
	}
}
