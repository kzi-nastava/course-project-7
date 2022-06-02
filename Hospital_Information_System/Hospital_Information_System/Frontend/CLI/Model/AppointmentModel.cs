using HospitalIS.Backend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HospitalIS.Backend.Controller;
using HospitalIS.Backend.Repository;
using HospitalIS.Frontend.CLI.View;

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

        private const string hintDesperateSearchRespectDoctorOnly = "Closest respecting only doctor";
        private const string hintDesperateSearchRespectIntervalOnly = "Closest respecting only time interval";
        private const string hintDesperateSearchRespectDateOnly = "Closest respecting only latest date";
        private const string hintDesperateSearchIgnoreDate = "Closest optimal when ignoring latest date";
        private const string hintDesperateSearchClosestOverall = "Closest overall";

        private const string askCreateAppointment = "Are you sure you want to create this appointment?";

        private const string hintMakeReferral =
            "If you want to make a referral - press 1, if not press anything else";
        private const string hintWritePrescription =
            "If you want to write a prescription - press 1, if not press anything else";

        internal static void CreateAppointment(string inputCancelString, UserAccount user)
        {
            try
            {
                Appointment appointment = InputAppointment(inputCancelString, AppointmentController.GetProperties(), user);
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
                allAppointments = AppointmentController.GetAppointments(PatientController.GetPatientFromPerson(user.Person));
            }
            
            if (user.Type == UserAccount.AccountType.DOCTOR)
            {
                allAppointments = AppointmentController.GetAppointments(DoctorController.GetDoctorFromPerson(user.Person));
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
                AppointmentSearchBundle sb = InputSearchBundle(inputCancelString, user);
                AppointmentController.AppointmentProperty priority = InputPrioritizedProperty(inputCancelString);

                Appointment appointment = GetRecommendedAppointment(sb, priority, inputCancelString);
                Console.WriteLine(appointment.ToString());
                Console.WriteLine(askCreateAppointment);
                if (EasyInput<bool>.YesNo(inputCancelString))
                {
                    AppointmentController.Create(appointment, user);
                }
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        internal static void CreateUrgentAppointment(string inputCancelString, UserAccount user)
        {
            try
            {
                Doctor.MedicineSpeciality speciality = ReferralModel.inputSpecialty(inputCancelString);
                if (!DoctorController.DoctorExistForSpecialty(speciality))
                    throw new NothingToSelectException();
                
                AppointmentSearchBundle sb = InputSearchBundleUrgent(inputCancelString, user);
                Appointment appointment = AppointmentController.FindUrgentAppointmentSlot(inputCancelString, sb, speciality, user);
                
                Console.WriteLine(appointment.ToString());
                Console.WriteLine(askCreateAppointment);
                if (EasyInput<bool>.YesNo(inputCancelString))
                {
                    AppointmentController.Create(appointment, user);
                }
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static AppointmentSearchBundle InputSearchBundle(string inputCancelString, UserAccount user)
        {
            Doctor doctor = InputDoctor(inputCancelString, null, user);
            Patient patient = InputPatient(inputCancelString, null, user);
            TimeSpan start = InputStartOfRange(inputCancelString);
            TimeSpan end = InputEndOfRange(start, inputCancelString);
            DateTime latestDate = InputLatestDate(inputCancelString);

            return new AppointmentSearchBundle(doctor, patient, start, end, latestDate); 
        }
        
        private static AppointmentSearchBundle InputSearchBundleUrgent(string inputCancelString, UserAccount user)
        {
            Doctor doctor = null;
            Patient patient = InputPatient(inputCancelString, null, user);
            DateTime latestDate = DateTime.Today;

            TimeSpan start = TimeSpan.FromHours(DateTime.Now.TimeOfDay.TotalHours);
            start = new TimeSpan(start.Hours, start.Minutes + 10, 0);

            TimeSpan end = new TimeSpan((start.Hours+2)%24, start.Minutes, 0);

            if (end > TimeSpan.FromHours(0) && end < TimeSpan.FromHours(2))
            {
                start = new TimeSpan(0, 0, 0);
                end = new TimeSpan(start.Hours + 2, 0, 0);
                latestDate = DateTime.Today.AddDays(1);
            }

            return new AppointmentSearchBundle(doctor, patient, start, end, latestDate, true); 
        }

        private static Appointment GetRecommendedAppointment(AppointmentSearchBundle sb, AppointmentController.AppointmentProperty priority, string inputCancelString)
        {
            return GetOptimalAppointment(sb) ?? GetPrioritizedAppointment(sb, priority) ?? GetDesperateAppointment(sb, inputCancelString);
        }

        private static Appointment GetOptimalAppointment(AppointmentSearchBundle sb)
        {
            return AppointmentController.FindRecommendedAppointment(sb);
        }
        private static Appointment GetPrioritizedAppointment(AppointmentSearchBundle sb, AppointmentController.AppointmentProperty priority)
        {
            Console.WriteLine(hintOptimalSearchFailed);

            AppointmentSearchBundle sbPrioritized;
            if (priority == AppointmentController.AppointmentProperty.DOCTOR)
            {
                sbPrioritized = AppointmentSearchBundle.IgnoreInterval(sb);
            }
            else
            {
                sbPrioritized = AppointmentSearchBundle.IgnoreDoctor(sb);
            }
            return AppointmentController.FindRecommendedAppointment(sbPrioritized);
        }

        private static Appointment GetDesperateAppointment(AppointmentSearchBundle sb, string inputCancelString)
        {
            Console.WriteLine(hintPrioritySearchFailed);

            var desperateAppointments = new List<Appointment>();
            var desperateAppointment = new Appointment();

            void processDesperate(Func<AppointmentSearchBundle, AppointmentSearchBundle> newSb, string hint)
            {
                desperateAppointment = AppointmentController.FindRecommendedAppointment(newSb(sb));
                if (desperateAppointment != null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(hint);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(desperateAppointment.ToString());
                    desperateAppointments.Add(desperateAppointment);
                }
            }

            // Closest respecting only doctor
            processDesperate(AppointmentSearchBundle.RespectOnlyDoctorAndPatient, hintDesperateSearchRespectDoctorOnly);

            // Closest respecting only time interval
            processDesperate(AppointmentSearchBundle.RespectOnlyIntervalAndPatient, hintDesperateSearchRespectIntervalOnly);

            // Closest respecting only latest date.
            processDesperate(AppointmentSearchBundle.RespectOnlyLatestDateAndPatient, hintDesperateSearchRespectDateOnly);

            // Closest optimal when ignoring latest date.
            processDesperate(AppointmentSearchBundle.IgnoreLatestDate, hintDesperateSearchIgnoreDate);

            // Closest overall.
            processDesperate(AppointmentSearchBundle.RespectOnlyPatient, hintDesperateSearchClosestOverall);

            Console.WriteLine(hintSelectAppointment);
            return EasyInput<Appointment>.Select(desperateAppointments, inputCancelString);
        }

        private static List<AppointmentController.AppointmentProperty> SelectModifiableProperties(string inputCancelString, UserAccount user)
        {
            Console.WriteLine(hintSelectProperties);
            return EasyInput<AppointmentController.AppointmentProperty>.SelectMultiple(
                AppointmentController.GetModifiableProperties(user),
                ap => ap.ToString(),
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
                return EasyInput<Doctor>.Select(DoctorController.GetAvailableDoctors(referenceAppointment),
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
                return EasyInput<Patient>.Select(PatientController.GetAvailablePatients(referenceAppointment), inputCancelString);
            }
        }

        private static Room InputExaminationRoom(string inputCancelString, Appointment referenceAppointment, UserAccount user)
        {
            // Patient and doctor cannot modify the Room property, however when creating an Appointment we can reach here.
            if (user.Type != UserAccount.AccountType.MANAGER)
            {
                return RoomController.GetRandomAvailableExaminationRoom(referenceAppointment);
            }
            else
            {
                Console.WriteLine(hintSelectExaminationRoom);
                return EasyInput<Room>.Select(RoomController.GetAvailableExaminationRooms(referenceAppointment), inputCancelString);
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
                    newSchedule => PatientController.IsAvailable(patient, newSchedule, patientReferenceAppointment),
                    newSchedule => DoctorController.IsAvailable(doctor, newSchedule, doctorReferenceAppointment),
                    newSchedule => RoomController.IsAvailable(room, newSchedule, roomReferenceAppointment),
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
                    ts => AppointmentSearchBundle.TsInDay(ts),
                    ts => AppointmentSearchBundle.TsZeroSeconds(ts),
                },
                new string[]
                {
                    AppointmentSearchBundle.ErrTimeSpanNotInDay,
                    AppointmentSearchBundle.ErrTimeSpanHasSeconds,
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
                    ts => AppointmentSearchBundle.TsInDay(ts),
                    ts => AppointmentSearchBundle.TsZeroSeconds(ts),
                    ts => AppointmentSearchBundle.TsIsAfter(ts, start),
                },
                new string[]
                {
                    AppointmentSearchBundle.ErrTimeSpanNotInDay,
                    AppointmentSearchBundle.ErrTimeSpanHasSeconds,
                    AppointmentSearchBundle.ErrEndBeforeStart,
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
                    dt => AppointmentSearchBundle.DtNotTooSoon(dt),
                },
                new string[]
                {
                    AppointmentSearchBundle.ErrDateTooSoon,
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
            Console.WriteLine(hintWritePrescription);
            option = Console.ReadLine();
            if (option == "1")
            {
                PrescriptionModel.CreatePrescription(inputCancelString, MedicalRecordController.GetPatientsMedicalRecord(appointmentToStart.Patient));
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
            return EasyInput<Doctor>.Select(DoctorController.GetDoctorsBySpecialty(speciality),
                inputCancelString);
        }
    }
}
