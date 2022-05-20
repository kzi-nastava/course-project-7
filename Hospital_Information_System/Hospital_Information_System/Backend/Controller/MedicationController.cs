using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Util;

namespace HospitalIS.Backend.Controller
{
	internal static class MedicationController
	{
		public static List<Medication> GetMedications()
		{
			return IS.Instance.Hospital.Medications.Where(meds => !meds.Deleted).ToList();
		}
	}
}
