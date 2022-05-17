using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Controller
{
    internal class PatientController
    {
        public static List<Patient> GetPatients()
        {
            return IS.Instance.Hospital.Patients.FindAll(p => !p.Deleted);
        }

        public static Patient GetPatientFromPerson(Person person)
        {
            return IS.Instance.Hospital.Patients.Find(p => p.Person == person);
        }

        public static List<Patient> GetAvailablePatients(Appointment refAppointment)
        {
            if (refAppointment == null)
            {
                return GetPatients();
            }

            return GetPatients().FindAll(d => IsAvailable(d, refAppointment.ScheduledFor, refAppointment));
        }

        public static bool IsAvailable(Patient Patient, DateTime newSchedule, Appointment refAppointment = null)
        {
            foreach (Appointment appointment in AppointmentController.GetAppointments())
            {
                if ((Patient == appointment.Patient) && (appointment != refAppointment))
                {
                    if (AppointmentController.AreColliding(appointment.ScheduledFor, newSchedule))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static Patient FindFirstAvailablePatient(DateTime scheduledFor)
        {
            return GetPatients().First(p => IsAvailable(p, scheduledFor));
        }
    }
}
