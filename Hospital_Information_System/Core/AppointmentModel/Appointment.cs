using System;
using HIS.Core.DoctorModel;
using HIS.Core.Foundation;
using HIS.Core.PatientModel;
using HIS.Core.RoomModel;
using Newtonsoft.Json;

namespace HIS.Core.AppointmentModel
{
    public enum AppointmentProperty
    {
        DOCTOR, PATIENT, ROOM, SCHEDULED_FOR, ANAMNESIS
    }

    public class Appointment : Entity
    {
        [JsonConverter(typeof(DoctorJSONReferenceConverter))]
        public Doctor Doctor { get; set; }

        [JsonConverter(typeof(PatientJSONReferenceConverter))]
        public Patient Patient { get; set; }

        [JsonConverter(typeof(RoomJSONReferenceConverter))]
        public Room Room { get; set; }

        public DateTime ScheduledFor { get; set; }

        public string Anamnesis { get; set; }

        public Appointment(Doctor doctor, Patient patient, Room room, DateTime scheduledFor, string anamnesis = "")
        {
            Doctor = doctor;
            Patient = patient;
            Room = room;
            ScheduledFor = scheduledFor;
            Anamnesis = anamnesis;
        }

        public Appointment()
        {
        }
    }
}
