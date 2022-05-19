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
			return GetMedicationRequests().Where(req => req.Feedback.Count() == 0 || req.Feedback[0].Reviewer == doctor).ToList();
		}
	}
}
