using Newtonsoft.Json;

namespace HospitalIS.Backend
{
    public class UpdateRequest : Request
    {
        [JsonConverter(typeof(Repository.AppointmentRepository.AppointmentReferenceConverter))]
        public Appointment OldAppointment { get; set; }

        // We serialize this Appointment by value.
        public Appointment NewAppointment { get; set; }

        public UpdateRequest(UserAccount requestee, Appointment oldAppointment, Appointment newAppointment) : base(requestee)
        {
            OldAppointment = oldAppointment;
            NewAppointment = newAppointment;
        }

        public override string ToString()
        {
            return $"UpdateRequest{{Id = {Id}, OldAppointmentId = {OldAppointment.Id}, NewAppointmentId = {NewAppointment.Id}, PatientId = {Requestee.Id}}}";
        }
    }
}
