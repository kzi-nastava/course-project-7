using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.MedicationModel.MedicationRequestModel
{
	public class MedicationReview
	{
		public string Message { get; set; }
		public DateTime Timestamp { get; set; }
		public MedicationRequestState Verdict { get; set; }
		// todo: reviewer

		public MedicationReview()
		{
		}

		public MedicationReview(string message, DateTime timestamp, MedicationRequestState verdict)
		{
			Message = message;
			Timestamp = timestamp;
			Verdict = verdict;
		}

		public override string ToString()
		{
			return $"MedicationReview{{Timestamp={Timestamp}, Verdict={Verdict}, Message={Message}}}";
		}
	}
}
