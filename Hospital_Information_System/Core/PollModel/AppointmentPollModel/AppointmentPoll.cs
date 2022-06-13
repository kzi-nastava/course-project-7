using HIS.Core.AppointmentModel;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HIS.Core.PollModel.AppointmentPollModel
{
    public class AppointmentPoll : Poll
    {
        [JsonConverter(typeof(AppointmentJSONReferenceConverter))]
        public Appointment Appointment { get; set; }

        public AppointmentPoll(Dictionary<string, int> questionnaire, string comment, Appointment appointment) : base(questionnaire, comment)
        {
            Appointment = appointment;
        }
    }
}
