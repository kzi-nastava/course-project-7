using System;

namespace HospitalIS.Backend
{
    public class DaysOffRequest : Entity
    {
        public enum DaysOffRequestState
        {
            SENT, APPROVED, REJECTED
        }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Reason { get; set; }
        public DaysOffRequestState State { get; set; }
        public string RejectionExplanation { get; set; }

        public DaysOffRequest()
        {
            
        }
        
        public DaysOffRequest(DateTime start, DateTime end, string reason, DaysOffRequestState state)
        {
            Start = start;
            End = end;
            Reason = reason;
            State = state;
            RejectionExplanation = "";
        }

        public DaysOffRequest(DateTime start, DateTime end, string reason, DaysOffRequestState state, string rejectionExplanation)
        {
            Start = start;
            End = end;
            Reason = reason;
            State = state;
            RejectionExplanation = rejectionExplanation;
        }

        public override string ToString()
        {
            if (State != DaysOffRequestState.REJECTED)
                return $"DaysOffRequest{{Start={Start}, End={End}, Reason={Reason}, State={State}}}";
            return $"DaysOffRequest{{Start={Start}, End={End}, Reason={Reason}, State={State}, RejectionExplanation={RejectionExplanation}}}";
        }
    }
}