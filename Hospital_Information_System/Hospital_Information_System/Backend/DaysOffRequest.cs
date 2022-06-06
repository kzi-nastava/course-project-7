using System;
using System.Text.Json.Serialization;

namespace HospitalIS.Backend
{
    public class DaysOffRequest : Entity
    {
        public enum DaysOffRequestState
        {
            SENT, APPROVED, REJECTED
        }
        
        [JsonConverter(typeof(Repository.DoctorRepository.DoctorReferenceConverter))]
        public Doctor Requester { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Reason { get; set; }
        public DaysOffRequestState State { get; set; }
        public string RejectionExplanation { get; set; }

        public DaysOffRequest()
        {
            
        }
        
        public DaysOffRequest(Doctor requester, DateTime start, DateTime end, string reason, DaysOffRequestState state)
        {
            Requester = requester;
            Start = start;
            End = end;
            Reason = reason;
            State = state;
            RejectionExplanation = "";
        }

        public DaysOffRequest(Doctor requester, DateTime start, DateTime end, string reason, DaysOffRequestState state, string rejectionExplanation)
        {
            Requester = requester;
            Start = start;
            End = end;
            Reason = reason;
            State = state;
            RejectionExplanation = rejectionExplanation;
        }

        public override string ToString()
        {
            if (State == DaysOffRequestState.REJECTED)
                return $"DaysOffRequest{{Doctor={Requester}, Start={Start}, End={End}, Reason={Reason}, State={State}, RejectionExplanation={RejectionExplanation}}}";
            return $"DaysOffRequest{{Doctor={Requester}, Start={Start}, End={End}, Reason={Reason}, State={State}}}";
        }
    }
}