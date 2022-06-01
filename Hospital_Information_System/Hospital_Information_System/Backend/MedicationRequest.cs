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
		public string Message {get; set;}
		public DateTime Timestamp {get; set;}
		
		[JsonConverter(typeof(Repository.DoctorRepository.DoctorReferenceConverter))]
		public Doctor Reviewer {get; set;}
		
		public MedicationRequestState Verdict {get; set;}
		
		public MedicationRequestReview() 
		{
		}

		public MedicationRequestReview(Doctor reviewer, string message, MedicationRequestState verdict) {
			Debug.Assert(verdict != MedicationRequestState.SENT);
			
			Reviewer = reviewer;
			Message = message;
			Timestamp = DateTime.Now;
			Verdict = verdict;
		}

		public override string ToString()
		{
			return $"RequestFeedback{{Timestamp={Timestamp}, Message={Message}, Verdict={Verdict}, Reviewer={Reviewer.ToString()}}}";
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
        
        
        /*
        public override string ToString()
        {
	        if (Reviews.Count != 0)
	        {
		        return $"MedicationRequest{{Medication={Medication.ToString()}, State={State}, Reviews=[\n{ConvertReviewListToString(Reviews)}\n]}}";
	        }
	        return $"MedicationRequest{{Medication={Medication.ToString()}, State={State}, Reviews=[None yet]}}";
        }
        
        
        private static string ConvertReviewListToString(List<MedicationRequestReview> entry)
        {
	        string ret = "";
	        for (int i = 0; i <= entry.Count - 1; i++)
	        {
		        ret += entry[i].ToString();
		        if (i < entry.Count - 1)
		        {
			        ret += ";\n ";
		        }
	        }

	        return ret;
        }
        */
        
    }
}