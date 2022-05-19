using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;

namespace HospitalIS.Backend
{
	public enum RequestState
	{
		REVISION, APPROVED, REJECTED
	}

	public class MedicationRequestFeedback
	{
		public string Message {get; private set;}
		public DateTime Timestamp {get; private set;}
		public RequestState State {get; private set;}

		[JsonConverter(typeof(Repository.DoctorRepository.DoctorReferenceConverter))]
		public Doctor Reviewer {get; private set;}
		
		public MedicationRequestFeedback() 
		{
		}

		public MedicationRequestFeedback(string message, RequestState state, Doctor reviewer) {
			Message = message;
			Timestamp = DateTime.Now;
			State = state;
			Reviewer = reviewer;
		}

		public override string ToString()
		{
			return $"RequestFeedback{{Timestamp={Timestamp}, Message={Message}, State={State}}}";
		}
	}

    public class MedicationRequest : Entity
    {
		public Medication Medication; // Kept internally until it gets approved.

		public List<MedicationRequestFeedback> Feedback = new List<MedicationRequestFeedback>();

        public MedicationRequest()
        {
        }

        public MedicationRequest(Medication medication) 
		{
			Medication = medication;
		}

        public override string ToString()
        {
            return $"MedicationRequest{{Medication={Medication.ToString()}, Feedback=[{Feedback.Select(f => f.ToString() + "\n")}]}}";
        }
    }
}