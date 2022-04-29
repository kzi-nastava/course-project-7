using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Controller
{
    internal class AppointmentController
    {
        public const int lengthOfAppointmentInMinutes = 15;
        public const int daysBeforeAppointmentUnmodifiable = 1;
        public const int daysBeforeModificationNeedsRequest = 2;

        public enum AppointmentProperty
        {
            DOCTOR,
            PATIENT,
            ROOM,
            SCHEDULED_FOR,
        }

        public static string GetAppointmentPropertyName(AppointmentProperty ap)
        {
            return Enum.GetName(typeof(AppointmentProperty), ap);
        }

        public static List<AppointmentProperty> GetModifiableProperties(UserAccount user)
        {
            List<AppointmentProperty> modifiableProperties = GetAllAppointmentProperties();

            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                modifiableProperties.Remove(AppointmentProperty.PATIENT);
                modifiableProperties.Remove(AppointmentProperty.ROOM);
            }

            return modifiableProperties;
        }

        public static List<AppointmentProperty> GetAllAppointmentProperties()
        {
            return Enum.GetValues(typeof(AppointmentProperty)).Cast<AppointmentProperty>().ToList();
        }

        public static List<Appointment> GetModifiableAppointments(UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                return IS.Instance.Hospital.Appointments.Where(
                    a => !a.Deleted && user.Person.Id == a.Patient.Person.Id && CanModifyAppointment(a.ScheduledFor, user)).ToList();
            }
            else
            {
                return IS.Instance.Hospital.Appointments.Where(
                    a => !a.Deleted && CanModifyAppointment(a.ScheduledFor, user)).ToList();
            }
        }

        public static List<Appointment> GetModifiableAppointments()
        {
            return IS.Instance.Hospital.Appointments.Where(a => !a.Deleted).ToList();
        }

        public static bool CanModifyAppointment(DateTime scheduledFor, UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                TimeSpan difference = scheduledFor - DateTime.Now;
                return difference.TotalDays >= daysBeforeAppointmentUnmodifiable;
            }
            else
            {
                return true;
            }
        }

        public static bool MustRequestAppointmentModification(DateTime scheduledFor, UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                TimeSpan difference = scheduledFor - DateTime.Now;
                return difference.TotalDays < daysBeforeModificationNeedsRequest;
            }
            else
            {
                return false;
            }
        }

        public static List<Doctor> GetModifiableDoctors()
        {
            return IS.Instance.Hospital.Doctors.Where(d => !d.Deleted).ToList();
        }

        public static List<Patient> GetModifiablePatients()
        {
            return IS.Instance.Hospital.Patients.Where(p => !p.Deleted).ToList();
        }

        public static List<Room> GetModifiableExaminationRooms()
        {
            return IS.Instance.Hospital.Rooms.Where(r => !r.Deleted && r.Type == Room.RoomType.EXAMINATION).ToList();
        }

        public static List<Doctor> GetAvailableDoctors(Appointment refAppointment)
        {
            if (refAppointment == null)
            {
                return GetModifiableDoctors();
            }

            return IS.Instance.Hospital.Doctors.Where(
                d => !d.Deleted && IsAvailable(d, refAppointment, refAppointment.ScheduledFor)).ToList();
        }

        public static List<Patient> GetAvailablePatients(Appointment refAppointment)
        {
            if (refAppointment == null)
            {
                return GetModifiablePatients();
            }

            return IS.Instance.Hospital.Patients.Where(
                p => !p.Deleted && IsAvailable(p, refAppointment, refAppointment.ScheduledFor)).ToList();
        }

        public static List<Room> GetAvailableExaminationRooms(Appointment refAppointment)
        {
            if (refAppointment == null)
            {
                return GetModifiableExaminationRooms();
            }

            return IS.Instance.Hospital.Rooms.Where(
                r => !r.Deleted && r.Type == Room.RoomType.EXAMINATION && IsAvailable(r, refAppointment, refAppointment.ScheduledFor)).ToList();
        }

        public static bool IsAvailable(Patient patient, Appointment refAppointment, DateTime newSchedule)
        {
            foreach (Appointment appointment in GetModifiableAppointments())
            {
                if ((patient == appointment.Patient) && (appointment != refAppointment))
                {
                    if (AreColliding(appointment.ScheduledFor, newSchedule))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsAvailable(Doctor doctor, Appointment refAppointment, DateTime newSchedule)
        {
            foreach (Appointment appointment in GetModifiableAppointments())
            {
                if ((doctor == appointment.Doctor) && (appointment != refAppointment))
                {
                    if (AreColliding(appointment.ScheduledFor, newSchedule))
                    {
                        return false;
                    }

                }
            }
            return true;
        }

        public static bool IsAvailable(Room room, Appointment refAppointment, DateTime newSchedule)
        {
            foreach (Appointment appointment in GetModifiableAppointments())
            {
                if ((room == appointment.Room) && (appointment != refAppointment))
                {
                    if (AreColliding(appointment.ScheduledFor, newSchedule))
                    {
                        return false;
                    }

                }
            }
            return true;
        }

        public static bool AreColliding(DateTime schedule1, DateTime schedule2)
        {
            TimeSpan difference = schedule1 - schedule2;
            return Math.Abs(difference.TotalMinutes) < lengthOfAppointmentInMinutes;
        }

        public static void CopyAppointment(Appointment target, Appointment source, List<AppointmentProperty> whichProperties)
        {
            if (whichProperties.Contains(AppointmentProperty.DOCTOR)) target.Doctor = source.Doctor;
            if (whichProperties.Contains(AppointmentProperty.PATIENT)) target.Patient = source.Patient;
            if (whichProperties.Contains(AppointmentProperty.SCHEDULED_FOR)) target.ScheduledFor = source.ScheduledFor;
        }
    }
}
