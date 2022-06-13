using HIS.Core.AppointmentModel;
using HIS.Core.UserAccountModel;
using Newtonsoft.Json;

namespace HIS.Core.ModificationRequestModel.DeleteRequestModel
{
    public class DeleteRequest : ModificationRequest
    {
        [JsonConverter(typeof(AppointmentJSONReferenceConverter))]
        public Appointment Appointment { get; set; }

        public DeleteRequest(UserAccount requestee, Appointment appointment) : base(requestee)
        {
            Appointment = appointment;
        }

        public override string ToString()
        {
            return $"DeleteRequest{{Id = {Id}, AppointmentId = {Appointment.Id}, PatientId = {Requestee.Id}}}";
        }
    }
}
