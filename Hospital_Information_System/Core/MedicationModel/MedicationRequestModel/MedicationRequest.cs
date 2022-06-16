using HIS.Core.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.MedicationModel.MedicationRequestModel
{
	public enum MedicationRequestState { SENT, RETURNED, APPROVED, REJECTED };

	public class MedicationRequest : Entity
	{
		public Medication Medication { get; set; }
		public MedicationRequestState State { get; set; } = MedicationRequestState.SENT;
		public IList<MedicationReview> Reviews = new List<MedicationReview>();
		

		public MedicationRequest()
		{
		}

		public MedicationRequest(Medication medication)
		{
			Medication = medication;
		}

		public override string ToString()
		{
			if (Reviews.Count != 0)
			{
				return $"Medication{{Medication={Medication}, State={State}, ReviewCount={Reviews.Count}, LastReview={Reviews.Last()}}}";
			}
			return
				$"Medication{{Medication={Medication}, State={State}, ReviewCount={Reviews.Count}, Reviews=None yet}}";
		}
	}
}
