using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;
using System.Diagnostics;

namespace HospitalIS.Backend
{
	public enum MedicationRequestState
	{
		SENT, RETURNED, APPROVED, REJECTED
	}	

	public class MedicationRequestReview
	{
		public string Message {get; private set;}
		public DateTime Timestamp {get; private set;}
		
		[JsonConverter(typeof(Repository.DoctorRepository.DoctorReferenceConverter))]
		public Doctor Reviewer {get; private set;}

		[JsonConverter(typeof(Repository.MedicationRepository.MedicationReferenceConverter))]
		public MedicationRequest Request {get; private set;}
		public MedicationRequestState Verdict {get; private set;}
		
		public MedicationRequestReview() 
		{
		}

		public MedicationRequestReview(MedicationRequest request, Doctor reviewer, string message, MedicationRequestState verdict) {
			Debug.Assert(verdict != MedicationRequestState.SENT);
			
			Reviewer = reviewer;
			Message = message;
			Timestamp = DateTime.Now;
			Verdict = verdict;
			Request = request;
		}

		public override string ToString()
		{
			return $"RequestFeedback{{Timestamp={Timestamp}, Message={Message}, Verdict={Verdict}, Reveiwer={Reviewer.ToString()}}}";
		}
	}

    public class MedicationRequest : Entity
    {
		public Medication Medication; // Kept internally until it gets approved.

		public List<MedicationRequestReview> Reviews = new List<MedicationRequestReview>();

		public MedicationRequestState State {get; set;} = MedicationRequestState.SENT;

        public MedicationRequest()
        {
        }

        public MedicationRequest(Medication medication) 
		{
			Medication = medication;
		}

        public override string ToString()
        {
            return $"MedicationRequest{{Medication={Medication.ToString()}, Reviews=[{Reviews.Select(f => f.ToString() + "\n")}], State={State}}}";
        }
    }
}