using HIS.Core.AppointmentModel;
using System;
using System.Collections.Generic;

namespace HIS.Core.PersonModel.PatientModel.PatientAvailability
{
    public interface IPatientAvailabilityService
    {
        public bool IsAvailable(Patient doctor, DateTime newSchedule, Appointment refAppointment = null);
        public Patient FindFirstAvailablePatient(DateTime scheduledFor);
        public IEnumerable<Patient> GetAvailable(Appointment refAppointment);
    }
}
