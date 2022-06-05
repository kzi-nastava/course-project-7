using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HospitalIS.Backend
{
    public class Appointment : Entity
    {
        [JsonConverter(typeof(Repository.DoctorRepository.DoctorReferenceConverter))]
        public Doctor Doctor { get; set; }

        [JsonConverter(typeof(Repository.PatientRepository.PatientReferenceConverter))]
        public Patient Patient { get; set; }

        [JsonConverter(typeof(Repository.RoomRepository.RoomReferenceConverter))]
        public Room Room { get; set; }

        public DateTime ScheduledFor { get; set; }
        
        public String Anamnesis { get; set; }

        public Appointment(Doctor doctor, Patient patient, Room room, DateTime scheduledFor, String anamnesis = "")
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

        public override string ToString()
        {
            return $"Appointment{{Id = {Id}, Doctor = {Doctor.Id}, Patient = {Patient.Id}, Room = {Room.Id}, ScheduledFor = {ScheduledFor}, Anamnesis = {Anamnesis}}}";
        }

        public string AnamnesisFocusedToString()
        {
            return $"Appointment{{Id = {Id}, Doctor = {Doctor.Id}, DoctorSpecialty = {Doctor.Specialty}, ScheduledFor = {ScheduledFor}, Anamnesis = {Anamnesis}}}";
        }

        public abstract class AppointmentComparer : Comparer<Appointment>
        {

        }

        public class CompareByDate : AppointmentComparer
        {
            public override int Compare([AllowNull] Appointment x, [AllowNull] Appointment y)
            {
                return x.ScheduledFor.CompareTo(y.ScheduledFor);
            }
        }

        public class CompareByDoctor : AppointmentComparer
        {
            public override int Compare([AllowNull] Appointment x, [AllowNull] Appointment y)
            {
                return x.Doctor.Id.CompareTo(y.Doctor.Id);
            }
        }

        public class CompareByDoctorSpecialty : AppointmentComparer
        {
            public override int Compare([AllowNull] Appointment x, [AllowNull] Appointment y)
            {
                return x.Doctor.Specialty.CompareTo(y.Doctor.Specialty);
            }
        }
    }
}
