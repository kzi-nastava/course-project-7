using System;
using HospitalIS.Backend;
using HospitalIS.Backend.Repository;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class DaysOffRequestModel
    {
        internal static void CreateDaysOffRequest()
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now.AddDays(3);
            string reason = "some reason";
            DaysOffRequest.DaysOffRequestState state = DaysOffRequest.DaysOffRequestState.SENT;
            DaysOffRequest daysOffRequest = new DaysOffRequest(start, end, reason, state);
            IS.Instance.DaysOffRequestRepo.Add(daysOffRequest);
        }
    }
}