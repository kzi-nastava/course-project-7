using System;
using System.Collections.Generic;
using System.Text;
using HIS.Core.PersonModel.DoctorModel;
using Newtonsoft.Json;

namespace HIS.Core.MedicationModel.MedicationRequestModel
{
	public class MedicationReview
	{
		public string Message { get; set; }
		public DateTime Timestamp { get; set; }
		public MedicationRequestState Verdict { get; set; }
		
		[JsonConverter(typeof(DoctorJSONReferenceConverter))]
		public Doctor Reviewer {get; set;}
		

		public MedicationReview()
		{
		}

		public MedicationReview(string message, DateTime timestamp, MedicationRequestState verdict, Doctor reviewer)
		{
			Message = message;
			Timestamp = timestamp;
			Verdict = verdict;
			Reviewer = reviewer;
		}

		public override string ToString()
		{
			return $"MedicationReview{{Timestamp={Timestamp}, Verdict={Verdict}, Message={Message}, Reviewer={Reviewer.ToString()}}}";
		}
	}
}
