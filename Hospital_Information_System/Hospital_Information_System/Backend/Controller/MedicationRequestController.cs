using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Util;

namespace HospitalIS.Backend.Controller
{
	internal static class MedicationRequestController
	{
		public static List<MedicationRequest> GetMedicationRequests()
		{
			return IS.Instance.Hospital.MedicationRequests.Where(ren => !ren.Deleted).ToList();
		}

		public static List<MedicationRequest> GetMedicationRequests(Doctor doctor)
		{
			return GetMedicationRequests().Where(req => req.State == MedicationRequestState.SENT && (req.Reviews.Count() == 0 || req.Reviews[0].Reviewer == doctor)).ToList();
		}

		public static List<MedicationRequest> GetRequestsForRevision()
		{
			return GetMedicationRequests().Where(req => req.State == MedicationRequestState.RETURNED).ToList();
		}

		public static List<MedicationRequest> GetPendingRequests()
		{
			return GetMedicationRequests().Where(req => req.State == MedicationRequestState.SENT || req.State == MedicationRequestState.RETURNED).ToList();
		}

		private static List<MedicationRequestState> GetAllRequestStates()
		{
			return Enum.GetValues(typeof(MedicationRequestState)).Cast<MedicationRequestState>().ToList();
		}

		public static List<MedicationRequestState> GetRequestStatesForDoctor()
		{
			List<MedicationRequestState> states = GetAllRequestStates();
			states.Remove(MedicationRequestState.SENT);
			return states;
		}

		public static void Accept(ref MedicationRequest request, ref MedicationRequestReview review)
		{
			request.State = MedicationRequestState.APPROVED;
			request.Reviews.Add(review);
			var newMedication = request.Medication;
			IS.Instance.MedicationRepo.Add(newMedication);
		}

		public static void Reject(ref MedicationRequest request, ref MedicationRequestReview review)
		{
			request.State = MedicationRequestState.REJECTED;
			request.Reviews.Add(review);
			IS.Instance.MedicationRequestRepo.Remove(request);
		}

		public static void SendForRevision(ref MedicationRequest request, ref MedicationRequestReview review)
		{
			request.State = MedicationRequestState.RETURNED;
			request.Reviews.Add(review);
		}
	}
}
