using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Controller
{
    internal class DoctorController
    {
        public static List<Doctor> GetDoctors()
        {
            return IS.Instance.Hospital.Doctors.FindAll(d => !d.Deleted);
        }

        public static Doctor GetDoctorFromPerson(Person person)
        {
            return IS.Instance.Hospital.Doctors.Find(d => d.Person == person);
        }

        public static List<Doctor> GetAvailableDoctors(Appointment refAppointment)
        {
            if (refAppointment == null)
            {
                return GetDoctors();
            }

            return GetDoctors().FindAll(d => IsAvailable(d, refAppointment.ScheduledFor, refAppointment));
        }
        
        
        public static List<Doctor.MedicineSpeciality> GetAllSpecialties()
        {
            return Enum.GetValues(typeof(Doctor.MedicineSpeciality)).Cast<Doctor.MedicineSpeciality>().ToList();
        }

        public static bool IsAvailable(Doctor doctor, DateTime newSchedule, Appointment refAppointment = null)
        {
            foreach (Appointment appointment in AppointmentController.GetAppointments())
            {
                if ((doctor == appointment.Doctor) && (appointment != refAppointment))
                {
                    if (AppointmentController.AreColliding(appointment.ScheduledFor, newSchedule))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static List<Doctor> GetDoctorsBySpecialty(Doctor.MedicineSpeciality speciality)
        {
            return GetDoctors().FindAll(d => d.Specialty == speciality);
        }

        public static bool DoctorExistForSpecialty(Doctor.MedicineSpeciality speciality)
        {
            return GetDoctors().Any(d => d.Specialty == speciality);
        }

        public static Doctor FindFirstAvailableDoctor(DateTime scheduledFor)
        {
            return GetDoctors().First(d => IsAvailable(d, scheduledFor));
        }

        public static Doctor FindFirstAvailableDoctorOfSpecialty(DateTime scheduledFor, Doctor.MedicineSpeciality speciality)
        {
            return GetDoctors().First(d => IsAvailable(d, scheduledFor) && d.Specialty == speciality);
        }

        private static List<Doctor> MatchByString(string query, Doctor.Comparer comparer, Func<Doctor, string> toStr)
        {
            var matches = GetDoctors().FindAll(d => toStr(d).Contains(query.Trim(), StringComparison.OrdinalIgnoreCase));
            matches.Sort(comparer);
            return matches;
        }

        public static List<Doctor> MatchByFirstName(string query, Doctor.Comparer comparer)
        {
            return MatchByString(query, comparer, d => d.Person.FirstName);
        }

        public static List<Doctor> MatchByLastName(string query, Doctor.Comparer comparer)
        {
            return MatchByString(query, comparer, d => d.Person.LastName);
        }

        public static List<Doctor> MatchBySpecialty(string query, Doctor.Comparer comparer)
        {
            return MatchByString(query, comparer, d => d.Specialty.ToString());
        }

        public static double CalculateRating(Doctor doctor)
        {
            return AppointmentRatingController.GetAppointmentRatings(doctor).Average(r => r.Rating);
        }
    }
}
