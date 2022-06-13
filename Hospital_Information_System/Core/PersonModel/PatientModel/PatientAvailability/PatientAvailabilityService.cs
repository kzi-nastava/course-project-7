using HIS.Core.AppointmentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.PersonModel.PatientModel.PatientAvailability
{
    public class PatientAvailabilityService : IPatientAvailabilityService
    {
        private IPatientService _patientService;
        private IAppointmentService _appointmentService;

        public PatientAvailabilityService()
        {
        }

        public PatientAvailabilityService(IPatientService doctorService, IAppointmentService appointmentService)
        {
            _patientService = doctorService;
            _appointmentService = appointmentService;
        }

        public Patient FindFirstAvailablePatient(DateTime scheduledFor)
        {
            return _patientService.GetAll().First(d => IsAvailable(d, scheduledFor));
        }

        public bool IsAvailable(Patient doctor, DateTime newSchedule, Appointment refAppointment = null)
        {
            foreach (Appointment appointment in _appointmentService.GetAll())
            {
                if ((doctor == appointment.Patient) && (appointment != refAppointment))
                {
                    if (_appointmentService.AreColliding(appointment.ScheduledFor, newSchedule))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public IEnumerable<Patient> GetAvailable(Appointment refAppointment)
        {
            if (refAppointment == null)
            {
                return _patientService.GetAll();
            }

            return _patientService.GetAll().Where(d => IsAvailable(d, refAppointment.ScheduledFor, refAppointment));
        }
    }
}
