using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Controller
{
    internal class AppointmentController
    {
        public const int LengthOfAppointmentInMinutes = 15;
        public const int DaysBeforeAppointmentUnmodifiable = 1;
        public const int DaysBeforeModificationNeedsRequest = 2;
        private const string hintRequestSent = "Request for modification sent.";


        public enum AppointmentProperty
        {
            DOCTOR,
            PATIENT,
            ROOM,
            SCHEDULED_FOR,
            ANAMNESIS
        }

        public static void Create(Appointment appointment, UserAccount user)
        {
            UserAccountController.AddCreatedAppointmentTimestamp(user, DateTime.Now);
            IS.Instance.AppointmentRepo.Add(appointment);
        }

        public static void Update(Appointment appointment, Appointment updatedAppointment, List<AppointmentProperty> propertiesToUpdate, UserAccount user)
        {
            UserAccountController.AddModifiedAppointmentTimestamp(user, DateTime.Now);
            if (MustRequestModification(appointment, user))
            {
                var proposedAppointment = new Appointment();
                CopyAppointment(proposedAppointment, appointment, GetAllAppointmentProperties());
                CopyAppointment(proposedAppointment, updatedAppointment, propertiesToUpdate);
                IS.Instance.UpdateRequestRepo.Add(new UpdateRequest(user, appointment, proposedAppointment));
                Console.WriteLine(hintRequestSent);
            }
            else
            {
                CopyAppointment(appointment, updatedAppointment, propertiesToUpdate);
            }
        }

        public static void Delete(Appointment appointment, UserAccount user)
        {
            UserAccountController.AddModifiedAppointmentTimestamp(user, DateTime.Now);
            if (MustRequestModification(appointment, user))
            {
                IS.Instance.DeleteRequestRepo.Add(new DeleteRequest(user, appointment));
                Console.WriteLine(hintRequestSent);
            }
            else
            {
                IS.Instance.AppointmentRepo.Remove(appointment);
            }
        }

        public static string GetName(AppointmentProperty ap)
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
                modifiableProperties.Remove(AppointmentProperty.ANAMNESIS);
            }
            
            if (user.Type == UserAccount.AccountType.DOCTOR)
            {
                modifiableProperties.Remove(AppointmentProperty.DOCTOR);
                modifiableProperties.Remove(AppointmentProperty.ROOM);
                modifiableProperties.Remove(AppointmentProperty.ANAMNESIS); //only allowed during an appointment
            }

            return modifiableProperties;
        }

        public static List<AppointmentProperty> GetAllAppointmentProperties()
        {
            return Enum.GetValues(typeof(AppointmentProperty)).Cast<AppointmentProperty>().ToList();
        }
        
        public static List<Appointment> GetAllPatientsAppointments(UserAccount user)
        {
            return IS.Instance.Hospital.Appointments.Where(
                a => !a.Deleted && user.Person.Id == a.Patient.Person.Id).ToList();

        }

        public static List<Appointment> GetAllDoctorsAppointments(UserAccount user)
        {
            return IS.Instance.Hospital.Appointments.Where(
                a => !a.Deleted && user.Person.Id == a.Doctor.Person.Id ).ToList();
        }

        public static List<Appointment> GetModifiableAppointments(UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                return IS.Instance.Hospital.Appointments.Where(
                    a => !a.Deleted && user.Person.Id == a.Patient.Person.Id && CanModify(a, user)).ToList();
            }
            
            if (user.Type == UserAccount.AccountType.DOCTOR)
            {
                return IS.Instance.Hospital.Appointments.Where(
                    a => !a.Deleted && user.Person.Id == a.Doctor.Person.Id && CanModify(a, user)).ToList();
            }
            else
            {
                return IS.Instance.Hospital.Appointments.Where(
                    a => !a.Deleted && CanModify(a, user)).ToList();
            }
        }

        
        public static List<Appointment> GetModifiableAppointments()
        {
            return IS.Instance.Hospital.Appointments.Where(a => !a.Deleted).ToList();
        }

        public static bool CanModify(Appointment appointment, UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                TimeSpan difference = appointment.ScheduledFor - DateTime.Now;
                return difference.TotalDays >= DaysBeforeAppointmentUnmodifiable;
            }
            else
            {
                return true;
            }
        }

        public static bool MustRequestModification(Appointment appointment, UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                TimeSpan difference = appointment.ScheduledFor - DateTime.Now;
                return difference.TotalDays < DaysBeforeModificationNeedsRequest;
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

        public static Room GetRandomAvailableExaminationRoom(Appointment refAppointment)
        {
            var rnd = new Random();
            var rooms = GetAvailableExaminationRooms(refAppointment);
            return rooms[rnd.Next(rooms.Count)];
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
            return Math.Abs(difference.TotalMinutes) < LengthOfAppointmentInMinutes;
        }

        public static void CopyAppointment(Appointment target, Appointment source, List<AppointmentProperty> whichProperties)
        {
            if (whichProperties.Contains(AppointmentProperty.DOCTOR)) target.Doctor = source.Doctor;
            if (whichProperties.Contains(AppointmentProperty.PATIENT)) target.Patient = source.Patient;
            if (whichProperties.Contains(AppointmentProperty.ROOM)) target.Room = source.Room;
            if (whichProperties.Contains(AppointmentProperty.SCHEDULED_FOR)) target.ScheduledFor = source.ScheduledFor;
            if (whichProperties.Contains(AppointmentProperty.ANAMNESIS)) target.Anamnesis = source.Anamnesis;
        }
    }
}
