using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Frontend.CLI;

namespace HospitalIS.Backend.Controller
{
    internal class AppointmentController
    {
        public const int LengthOfAppointmentInMinutes = 15;
        public const int DaysBeforeAppointmentUnmodifiable = 1;
        public const int DaysBeforeModificationNeedsRequest = 2;
        private const string hintRequestSent = "Request for modification sent.";
        private const string hintDateTimeNotInFuture = "Date and time must be in the future, try something else";
        private const string hintInputDate = "Input the date for which you want to be given scheduled appointments";
        private const string hintSelectAppointment = "Select appointment";
        
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
            MedicalRecord patientsRecord = MedicalRecordController.GetPatientsMedicalRecord(appointment.Patient);
            patientsRecord.Examinations.Add(appointment);
            IS.Instance.AppointmentRepo.Add(appointment);
        }

        public static void Update(Appointment appointment, Appointment updatedAppointment, List<AppointmentProperty> propertiesToUpdate, UserAccount user)
        {
            UserAccountController.AddModifiedAppointmentTimestamp(user, DateTime.Now);
            if (MustRequestModification(appointment, user))
            {
                var proposedAppointment = new Appointment();
                CopyAppointment(proposedAppointment, appointment, GetProperties());
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
                MedicalRecord patientsRecord = MedicalRecordController.GetPatientsMedicalRecord(appointment.Patient);
                patientsRecord.Examinations.Remove(appointment);
            }
        }

        public static List<Appointment> GetAppointments()
		{
            return IS.Instance.Hospital.Appointments.FindAll(ap => !ap.Deleted);
		}

        public static List<AppointmentProperty> GetModifiableProperties(UserAccount user)
        {
            List<AppointmentProperty> modifiableProperties = GetProperties();

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

        public static List<AppointmentProperty> GetProperties()
        {
            return Enum.GetValues(typeof(AppointmentProperty)).Cast<AppointmentProperty>().ToList();
        }

        public static List<AppointmentProperty> GetPrioritizableProperties()
        {
            return GetProperties().FindAll(ap => ap == AppointmentProperty.DOCTOR || ap == AppointmentProperty.SCHEDULED_FOR);
        }
        
        public static List<Appointment> GetPatientsAppointments(Patient patient)
        {
            return GetAppointments().FindAll(a => a.Patient == patient);
        }

        public static List<Appointment> GetDoctorsAppointments(Doctor doctor)
        {
            return GetAppointments().FindAll(a => a.Doctor == doctor);
        }
        
        public static List<Appointment> GetNextDoctorsAppointments(UserAccount user, string inputCancelString)
        {
            Console.WriteLine(hintInputDate);
            
            DateTime firstRelevantDay = EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>>()
                {
                    newSchedule => newSchedule.CompareTo(DateTime.Now) > 0,
                },
                new string[]
                {
                    hintDateTimeNotInFuture,
                },
                inputCancelString);
            var lastRelevantDay = firstRelevantDay.AddDays(3);
            List<Appointment> allAppointments = GetDoctorsAppointments(DoctorController.GetDoctorFromPerson(user.Person));
            List<Appointment> nextAppointments = new List<Appointment>();
            foreach (var appointment in allAppointments)
            {
                if (DateTime.Compare(firstRelevantDay, appointment.ScheduledFor) <= 0 && //if date of the appointment is later than the first relevant day
                    DateTime.Compare(appointment.ScheduledFor, lastRelevantDay) <= 0) // if date of the appointment is earlier than the last relevant day
                {
                    nextAppointments.Add(appointment);
                }
            }

            return nextAppointments;
        }

        public static List<Appointment> GetModifiableAppointments(UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                return GetAppointments().FindAll(a => a.Patient.Person == user.Person && CanModify(a, user));
            }
            
            if (user.Type == UserAccount.AccountType.DOCTOR)
            {
                return GetAppointments().FindAll(a => a.Doctor.Person == user.Person && CanModify(a, user));
            }
            else
            {
                return GetAppointments().FindAll(a => CanModify(a, user));
            }
        }

        private static List<Appointment> GetFirstFiveModifiableAppointments()
        {
            List<Appointment> modifiableAppointments = GetAppointments().Where(a => CanModify(a, new UserAccount(UserAccount.AccountType.SECRETARY)) && a.ScheduledFor == DateTime.Now).ToList();
            return modifiableAppointments.OrderBy(a => a.ScheduledFor).ToList().Take(5).ToList();
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

        public static Appointment FindRecommendedAppointment(AppointmentSearchBundle sb)
        {
            for (DateTime currDt = DateTime.Today; currDt < sb.By.Date; currDt = currDt.AddDays(1))
            {
                for (TimeSpan currTs = sb.Start; currTs <= sb.End; currTs = currTs.Add(TimeSpan.FromMinutes(1)))
                {
                    DateTime scheduledFor = currDt.Add(currTs);
                    if (scheduledFor < DateTime.Now) continue;

                    Doctor doctor = (sb.Doctor != null) ?
                        (DoctorController.IsAvailable(sb.Doctor, scheduledFor) ? sb.Doctor : null) : DoctorController.FindFirstAvailableDoctor(scheduledFor);
                    if (doctor == null) continue;

                    Patient patient = (sb.Patient != null) ?
                        (PatientController.IsAvailable(sb.Patient, scheduledFor) ? sb.Patient : null) : PatientController.FindFirstAvailablePatient(scheduledFor);
                    if (patient == null) continue;

                    Room room = RoomController.FindFirstAvailableExaminationRoom(scheduledFor);
                    if (room == null) continue;

                    return new Appointment(doctor, patient, room, scheduledFor);
                }
            }
            return null;
        }

        public static Appointment FindUrgentAppointmentSlot(string inputCancelString, AppointmentSearchBundle sb, Doctor.MedicineSpeciality speciality, UserAccount user)
        {
            DateTime currDt = sb.By;
            Doctor doctor = sb.Doctor;
            Patient patient = sb.Patient;

            for (TimeSpan currTs = sb.Start; currTs <= sb.End; currTs = currTs.Add(TimeSpan.FromMinutes(1)))
            {
                DateTime scheduledFor = currDt.Add(currTs);
                if (scheduledFor < DateTime.Now) continue;

                doctor = (sb.Doctor != null) ?
                    (DoctorController.IsAvailable(sb.Doctor, scheduledFor) ? sb.Doctor : null) : DoctorController.FindFirstAvailableDoctorOfSpecialty(scheduledFor, speciality);
                if (doctor == null) continue;

                patient = (sb.Patient != null) ?
                    (PatientController.IsAvailable(sb.Patient, scheduledFor) ? sb.Patient : null) : PatientController.FindFirstAvailablePatient(scheduledFor);
                if (patient == null) continue;

                Room room = RoomController.FindFirstAvailableExaminationRoom(scheduledFor);
                if (room == null) continue;

                return new Appointment(doctor, patient, room, scheduledFor);
            }
            
            Appointment rescheduledAppointment = RescheduleAppointment(inputCancelString, user);
            return new Appointment(doctor, patient, rescheduledAppointment.Room, rescheduledAppointment.ScheduledFor);
        }

        private static Appointment RescheduleAppointment(string inputCancelString, UserAccount user)
        {
            Appointment appointment = SelectAppointmentToReschedule(inputCancelString);
            
            AppointmentSearchBundle sb = new AppointmentSearchBundle(appointment.Doctor, appointment.Patient, AppointmentSearchBundle.DefaultStart, AppointmentSearchBundle.DefaultEnd, DateTime.Today);
            Appointment newAppointment = FindRecommendedAppointment(sb);
            
            AppointmentController.Create(newAppointment, user);
            AppointmentController.Delete(appointment, user);
            
            return appointment;
        }
        
        private static Appointment SelectAppointmentToReschedule(string inputCancelString)
        {
            Console.WriteLine(hintSelectAppointment);
            return EasyInput<Appointment>.Select(
                GetFirstFiveModifiableAppointments(), inputCancelString);
        }
    }
}
