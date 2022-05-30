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
			return GetMedicationRequests().Where(req => req.Reviews.Count() == 0 || req.Reviews[0].Reviewer == doctor).ToList();
		}

		public static List<MedicationRequest> GetRequestsForRevision()
		{
			return GetMedicationRequests().Where(req => req.State == MedicationRequestState.RETURNED).ToList();
		}
	}
}
