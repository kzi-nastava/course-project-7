using HospitalIS.Backend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HospitalIS.Backend.Controller;
using HospitalIS.Backend.Repository;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class AppointmentModel
    {
        private const string hintSelectAppointments = "Select appointments by their number, separated by whitespace.\nEnter a newline to finish";
        private const string hintSelectAppointment = "Select appointment";
        private const string hintSelectProperties = "Select properties by their number, separated by whitespace.\nEnter a newline to finish";
        private const string hintSelectDoctor = "Select doctor for the appointment";
        private const string hintSelectPatient = "Select patient for the appointment";
        private const string hintSelectExaminationRoom = "Select examination room for the appointment";
        private const string hintGetScheduledFor = "Enter date and time for the appointment";
        private const string hintPatientNotAvailable = "Patient is not available at the selected date and time";
        private const string hintDoctorNotAvailable = "Doctor is not available at the selected date and time";
        private const string hintExaminationRoomNotAvailable = "Examination room is not available at the selected date and time";
        private const string hintDateTimeNotInFuture = "Date and time must be in the future";
        private const string hintAppointmentScheduled = "You've successfully scheduled an appointment!";
        private const string hintAppointmentUpdated = "You've successfully updated an appointment!";
        private const string hintAppointmentDeleted = "You've successfully deleted an appointment!";
        private const string hintDeletedPatient = "Patient is deleted!";
        private const string hintNoScheduledAppoinments =
            "You don't have any scheduled appointments for the time given.";

        private const string hintCheckStartingAppointment =
            "If you want to start any appointment - press 1, if not press anything else";

        private const string hintAppointmentIsOver = "Appointment is over.";

        private const string hintGetStartOfRange = "Enter start of range";
        private const string hintGetEndOfRange = "Enter end of range";
        private const string hintGetLatestDesiredDate = "Enter latest desired date";
        private const string hintGetPrioritizedProperty = "Enter the prioritized property";
        private const string hintOptimalSearchFailed = "Could not find optimal appointment. Trying priority search...";
        private const string hintPrioritySearchFailed = "Could not find appointment using priority search. Showing closest matching appointments...";
        private const string hintDesperateSearchClosestOptimal = "Closest optimal when ignoring latest date";
        private const string hintDesperateSearchClosestOverall = "Closest overall";

        private const string hintMakeReferral =
            "If you want to make a referral - press 1, if not press anything else";

        internal static void CreateAppointment(string inputCancelString, UserAccount user)
        {
            try
            {
                Appointment appointment = InputAppointment(inputCancelString, AppointmentController.GetAllAppointmentProperties(), user);
                AppointmentController.Create(appointment, user);
                Console.WriteLine(hintAppointmentScheduled);
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        internal static void ReadAppointments(UserAccount user)
        {
            List<Appointment> allAppointments = new List<Appointment>();
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                allAppointments = AppointmentController.GetAllPatientsAppointments(user);
            }
            
            if (user.Type == UserAccount.AccountType.DOCTOR)
            {
                allAppointments = AppointmentController.GetAllDoctorsAppointments(user);
            }

            
            for (int i = 0; i < allAppointments.Count; i++)
            {
                var appointment = allAppointments[i];
                Console.WriteLine(appointment.ToString());
            }

        }

        internal static void UpdateAppointment(string inputCancelString, UserAccount user)
        {
            try
            {
                Appointment appointment = SelectModifiableAppointment(inputCancelString, user);
                var propertiesToUpdate = SelectModifiableProperties(inputCancelString, user);
                Appointment updatedAppointment = InputAppointment(inputCancelString, propertiesToUpdate, user, appointment);
                AppointmentController.Update(appointment, updatedAppointment, propertiesToUpdate, user);
                Console.WriteLine(hintAppointmentUpdated);
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal static void DeleteAppointment(string inputCancelString, UserAccount user)
        {
            try
            {
                var appointmentsToDelete = SelectModifiableAppointments(inputCancelString, user);
                foreach (Appointment appointment in appointmentsToDelete)
                {
                    AppointmentController.Delete(appointment, user);
                    Console.WriteLine(hintAppointmentDeleted);
                }
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal static void CreateRecommendedAppointment(string inputCancelString, UserAccount user)
        {
            try
            {
                AppointmentController.SearchBundle sb = InputSearchBundle(inputCancelString, user);
                AppointmentController.AppointmentProperty priority = InputPrioritizedProperty(inputCancelString);

                Appointment appointment = GetRecommendedAppointment(sb, priority, inputCancelString);
                AppointmentController.Create(appointment, user);
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static AppointmentController.SearchBundle InputSearchBundle(string inputCancelString, UserAccount user)
        {
            Doctor doctor = InputDoctor(inputCancelString, null, user);
            Patient patient = InputPatient(inputCancelString, null, user);
            TimeSpan start = InputStartOfRange(inputCancelString);
            TimeSpan end = InputEndOfRange(start, inputCancelString);
            DateTime latestDate = InputLatestDate(inputCancelString);

            return new AppointmentController.SearchBundle(doctor, patient, start, end, latestDate); 
        }

        private static Appointment GetRecommendedAppointment(AppointmentController.SearchBundle sb, AppointmentController.AppointmentProperty priority, string inputCancelString)
        {
            return GetOptimalAppointment(sb) ?? GetPrioritizedAppointment(sb, priority) ?? GetDesperateAppointment(sb, inputCancelString);
        }

        private static Appointment GetOptimalAppointment(AppointmentController.SearchBundle sb)
        {
            return AppointmentController.FindRecommendedAppointment(sb);
        }
        private static Appointment GetPrioritizedAppointment(AppointmentController.SearchBundle sb, AppointmentController.AppointmentProperty priority)
        {
            Console.WriteLine(hintOptimalSearchFailed);
            var sbPrioritized = new AppointmentController.SearchBundle(sb);
            if (priority == AppointmentController.AppointmentProperty.DOCTOR)
            {
                sbPrioritized.Start = TimeSpan.FromHours(0);
                sbPrioritized.End = TimeSpan.FromHours(24) - TimeSpan.FromMinutes(1);
            }
            else
            {
                sbPrioritized.Doctor = null;
            }
            return AppointmentController.FindRecommendedAppointment(sbPrioritized);
        }

        private static Appointment GetDesperateAppointment(AppointmentController.SearchBundle sb, string inputCancelString)
        {
            Console.WriteLine(hintPrioritySearchFailed);

            var desperateAppointments = new List<Appointment>();

            // Closest optimal when ignoring latest date.
            var sbDesperate = new AppointmentController.SearchBundle(sb)
            {
                By = DateTime.MaxValue
            };
            Appointment desperateAppointment = AppointmentController.FindRecommendedAppointment(sbDesperate);
            if (desperateAppointment != null)
            {
                Console.WriteLine(hintDesperateSearchClosestOptimal);
                Console.WriteLine(desperateAppointment.ToString());
                desperateAppointments.Add(desperateAppointment);
            }

            // Closest overall.
            sbDesperate = new AppointmentController.SearchBundle(sb)
            {
                Start = TimeSpan.FromHours(0),
                End = TimeSpan.FromHours(24) - TimeSpan.FromMinutes(1),
                Doctor = null,
                By = DateTime.MaxValue
            };
            desperateAppointment = AppointmentController.FindRecommendedAppointment(sbDesperate);
            if (desperateAppointment != null)
            {
                Console.WriteLine(hintDesperateSearchClosestOverall);
                Console.WriteLine(desperateAppointment.ToString());
                desperateAppointments.Add(desperateAppointment);
            }

            Console.WriteLine(hintSelectAppointment);
            return EasyInput<Appointment>.Select(desperateAppointments, inputCancelString);
        }

        private static List<AppointmentController.AppointmentProperty> SelectModifiableProperties(string inputCancelString, UserAccount user)
        {
            Console.WriteLine(hintSelectProperties);
            return EasyInput<AppointmentController.AppointmentProperty>.SelectMultiple(
                AppointmentController.GetModifiableProperties(user),
                ap => AppointmentController.GetName(ap),
                inputCancelString
            ).ToList();
        }

        private static Appointment SelectModifiableAppointment(string inputCancelString, UserAccount user)
        {
            Console.WriteLine(hintSelectAppointment);
            return EasyInput<Appointment>.Select(
                AppointmentController.GetModifiableAppointments(user), inputCancelString);
        }

        private static List<Appointment> SelectModifiableAppointments(string inputCancelString, UserAccount user)
        {
            Console.WriteLine(hintSelectAppointments);
            return EasyInput<Appointment>.SelectMultiple(
                AppointmentController.GetModifiableAppointments(user), inputCancelString).ToList();
        }

        private static Appointment InputAppointment(string inputCancelString, List<AppointmentController.AppointmentProperty> whichProperties, UserAccount user, Appointment refAppointment = null)
        {
            var appointment = new Appointment();

            if (whichProperties.Contains(AppointmentController.AppointmentProperty.DOCTOR))
            {
                appointment.Doctor = InputDoctor(inputCancelString, refAppointment, user);
            }

            if (whichProperties.Contains(AppointmentController.AppointmentProperty.PATIENT))
            {
                appointment.Patient = InputPatient(inputCancelString, refAppointment, user);
            }

            if (whichProperties.Contains(AppointmentController.AppointmentProperty.ROOM))
            {
                appointment.Room = InputExaminationRoom(inputCancelString, refAppointment, user);
            }

            if (whichProperties.Contains(AppointmentController.AppointmentProperty.SCHEDULED_FOR))
            {
                appointment.ScheduledFor = InputScheduledFor(inputCancelString, appointment, refAppointment);
            }

            return appointment;
        }

        private static Doctor InputDoctor(string inputCancelString, Appointment referenceAppointment, UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.DOCTOR)
            {
                return IS.Instance.Hospital.Doctors.First(d => d.Person.Id == user.Person.Id);
            }
            else
            {
                Console.WriteLine(hintSelectDoctor);
                return EasyInput<Doctor>.Select(AppointmentController.GetAvailableDoctors(referenceAppointment),
                    inputCancelString);
            }
        }
            

        private static Patient InputPatient(string inputCancelString, Appointment referenceAppointment, UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                return IS.Instance.Hospital.Patients.First(p => p.Person.Id == user.Person.Id);
            }
            else
            {
                Console.WriteLine(hintSelectPatient);
                return EasyInput<Patient>.Select(AppointmentController.GetAvailablePatients(referenceAppointment), inputCancelString);
            }
        }

        private static Room InputExaminationRoom(string inputCancelString, Appointment referenceAppointment, UserAccount user)
        {
            // Patient and doctor cannot modify the Room property, however when creating an Appointment we can reach here.
            if (user.Type != UserAccount.AccountType.MANAGER)
            {
                return AppointmentController.GetRandomAvailableExaminationRoom(referenceAppointment);
            }
            else
            {
                Console.WriteLine(hintSelectExaminationRoom);
                return EasyInput<Room>.Select(AppointmentController.GetAvailableExaminationRooms(referenceAppointment), inputCancelString);
            }
        }

        private static DateTime InputScheduledFor(string inputCancelString, Appointment proposedAppointment, Appointment referenceAppointment)
        {
            // If referenceAppointment is null -> we're doing a Create, proposedAppointment has non-null Patient and Doctor
            // If proposedAppointment's Patient and/or Doctor are null -> we're doing an Update, referenceAppointment has non-null Patient and Doctor
            // If the Patient/Doctor have been changed from the non-null referenceAppointment (we're Updating), then the referenceAppointment is no longer valid

            Doctor doctor = proposedAppointment.Doctor ?? referenceAppointment.Doctor;
            Appointment doctorReferenceAppointment = referenceAppointment;
            if ((proposedAppointment.Doctor != null) && (proposedAppointment.Doctor != referenceAppointment?.Doctor))
            {
                doctorReferenceAppointment = null;
            }

            Patient patient = proposedAppointment.Patient ?? referenceAppointment.Patient;
            Appointment patientReferenceAppointment = referenceAppointment;
            if ((proposedAppointment.Patient != null) && (proposedAppointment.Patient != referenceAppointment?.Patient))
            {
                patientReferenceAppointment = null;
            }

            Room room = proposedAppointment.Room ?? referenceAppointment.Room;
            Appointment roomReferenceAppointment = referenceAppointment;
            if ((proposedAppointment.Room != null) && (proposedAppointment.Room != referenceAppointment?.Room))
            {
                roomReferenceAppointment = null;
            }

            Console.WriteLine(hintGetScheduledFor);
            return EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>>()
                {
                    newSchedule => newSchedule.CompareTo(DateTime.Now) > 0,
                    newSchedule => AppointmentController.IsAvailable(patient, patientReferenceAppointment, newSchedule),
                    newSchedule => AppointmentController.IsAvailable(doctor, doctorReferenceAppointment, newSchedule),
                    newSchedule => AppointmentController.IsAvailable(room, roomReferenceAppointment, newSchedule),
                },
                new string[]
                {
                    hintDateTimeNotInFuture,
                    hintPatientNotAvailable,
                    hintDoctorNotAvailable,
                    hintExaminationRoomNotAvailable,
                },
                inputCancelString);
        }

        private static TimeSpan InputStartOfRange(string inputCancelString)
        {
            Console.WriteLine(hintGetStartOfRange);
            return EasyInput<TimeSpan>.Get(
                new List<Func<TimeSpan, bool>>()
                {
                    ts => AppointmentController.SearchBundle.TsInDay(ts),
                    ts => AppointmentController.SearchBundle.TsZeroSeconds(ts),
                },
                new string[]
                {
                    AppointmentController.SearchBundle.ErrTimeSpanNotInDay,
                    AppointmentController.SearchBundle.ErrTimeSpanHasSeconds,
                },
                inputCancelString,
                TimeSpan.Parse);
        }

        private static TimeSpan InputEndOfRange(TimeSpan start, string inputCancelString)
        {
            Console.WriteLine(hintGetEndOfRange);
            return EasyInput<TimeSpan>.Get(
                new List<Func<TimeSpan, bool>>()
                {
                    ts => AppointmentController.SearchBundle.TsInDay(ts),
                    ts => AppointmentController.SearchBundle.TsZeroSeconds(ts),
                    ts => AppointmentController.SearchBundle.TsIsAfter(ts, start),
                },
                new string[]
                {
                    AppointmentController.SearchBundle.ErrTimeSpanNotInDay,
                    AppointmentController.SearchBundle.ErrTimeSpanHasSeconds,
                    AppointmentController.SearchBundle.ErrEndBeforeStart,
                },
                inputCancelString,
                TimeSpan.Parse);
        }

        private static DateTime InputLatestDate(string inputCancelString)
        {
            Console.WriteLine(hintGetLatestDesiredDate);
            return EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>>()
                {
                    dt => AppointmentController.SearchBundle.DtNotTooSoon(dt),
                },
                new string[]
                {
                    AppointmentController.SearchBundle.ErrDateTooSoon,
                },
                inputCancelString);
        }

        private static AppointmentController.AppointmentProperty InputPrioritizedProperty(string inputCancelString)
        {
            Console.WriteLine(hintGetPrioritizedProperty);
            return EasyInput<AppointmentController.AppointmentProperty>.Select(
                AppointmentController.GetPrioritizableProperties(), inputCancelString);
        }

        public static void ShowNextAppointments(UserAccount user, string inputCancelString)
        {
            List <Appointment> nextAppointments = AppointmentController.GetNextDoctorsAppointments(user, inputCancelString);
            if (nextAppointments.Count == 0)
            {
                Console.WriteLine(hintNoScheduledAppoinments);
            }
            else
            {
                for (int i = 0; i < nextAppointments.Count; i++)
                {
                    var appointment = nextAppointments[i];

                    Console.WriteLine(appointment.ToString());
                    MedicalRecordModel.ReadMedicalRecord(appointment, inputCancelString);
                    Console.WriteLine("=====================================");
                }
                CheckIfDoctorWantsToStartAppointment(user, inputCancelString, nextAppointments);
            }
        }
        
        public static void CheckIfDoctorWantsToStartAppointment(UserAccount user, string inputCancelString, List<Appointment> startableAppointments)
        {
            Console.WriteLine(hintCheckStartingAppointment);
            string doctorsWill = Console.ReadLine();
            if (doctorsWill == "1")
            {
                StartAppointment(user, inputCancelString, startableAppointments);
            }
        }

        private static void StartAppointment(UserAccount user, string inputCancelString, List<Appointment> startableAppointments)
        {
            Console.WriteLine(hintSelectAppointment);
            var appointmentToStart = EasyInput<Appointment>.Select(startableAppointments, inputCancelString);
            MedicalRecordModel.UpdateMedicalRecordAndAnamnesis(appointmentToStart, inputCancelString);
            Console.WriteLine(hintMakeReferral);
            var option = Console.ReadLine();
            if (option == "1")
            {
                ReferralModel.CreateReferral(appointmentToStart, inputCancelString);
            }
            Console.WriteLine(hintAppointmentIsOver);
        }
        
        internal static void CreateAppointmentWithReferral(Referral referral, string inputCancelString, UserAccount user)
        {
            try
            {
                if (referral.Patient.Deleted)
                {
                    Console.WriteLine(hintDeletedPatient);
                }
                if (referral.Doctor == null)
                {
                    Doctor doctor = SelectDoctorBySpecialty(inputCancelString, referral.Specialty);
                    Console.WriteLine(hintSelectDoctor);
                    referral.Doctor = doctor;

                }
                
                Appointment appointment = SelectAppointment(referral, inputCancelString, user);
                AppointmentController.Create(appointment, user);
                Console.WriteLine(hintAppointmentScheduled);
                ReferralRepository.Scheduled(referral);
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        private static Appointment SelectAppointment(Referral referral, string inputCancelString, UserAccount user)
        {
            var appointment = new Appointment();
            
            appointment.Doctor = referral.Doctor;
            appointment.Patient = referral.Patient;
            appointment.Room = InputExaminationRoom(inputCancelString, null, user);
            appointment.ScheduledFor = InputScheduledFor(inputCancelString, appointment, null);

            return appointment;
        }

        private static Doctor SelectDoctorBySpecialty(string inputCancelString, Doctor.MedicineSpeciality speciality)
        {
            return EasyInput<Doctor>.Select(AppointmentController.GetAvailableDoctorsBySpecialty(speciality),
                inputCancelString);
        }
    }
}
