﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalIS.Backend
{
    public class Appointment : Entity
    {
        [JsonConverter(typeof(Repository.DoctorRepository.DoctorReferenceConverter))]
        public Doctor Doctor { get; set; }

        [JsonConverter(typeof(Repository.PatientRepository.PatientReferenceConverter))]
        public Patient Patient { get; set; }

        public DateTime ScheduledFor { get; set; }

        public Appointment(Doctor doctor, Patient patient, DateTime scheduledFor)
        {
            Doctor = doctor;
            Patient = patient;
            ScheduledFor = scheduledFor;
        }

        public Appointment()
        {
        }
        public override string ToString()
        {
            return $"Appointment{{Id = {Id}, Doctor = {Doctor.Id}, Patient = {Patient.Id}, ScheduledFor = {ScheduledFor}}}";
        }
    }
}
