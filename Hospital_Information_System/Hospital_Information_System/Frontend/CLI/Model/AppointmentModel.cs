using HospitalIS.Backend;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class AppointmentModel
    {
        private const string hintSelectAppointments = "Select appointments by their number, separated by whitespace.\nEnter a newline to finish";
        private const string hintSelectAppointment = "Select appointment";
        private const string hintSelectProperties = "Select properties by their number, separated by whitespace.\nEnter a newline to finish";
        private const string hintInputDoctor = "Select doctor for the appointment";
        private const string hintInputPatient = "Select patient for the appointment";
        private const string hintInputScheduledFor = "Enter date and time for the appointment";
        private const string hintPatientNotAvailable = "Patient is not available at the selected date and time";
        private const string hintDoctorNotAvailable = "Doctor is not available at the selected date and time";
        private const string hintDateTimeNotInFuture = "Date and time must be in the future";
        private const string hintAppointmentsNotAvailable = "No appointments available.";
        private const string errMsgNoDoctorAvailable = "No valid doctor available";
        private const string errMsgNoPatientAvailable = "No valid patient available";

        private const int lengthOfAppointmentInMinutes = 15;
        private const int daysBeforeAppointmentUnmodifiable = 1;

        internal class FailedInputAppointmentException : Exception
        {
            internal FailedInputAppointmentException(string errorMessage) : base($"Appointment input failed: {errorMessage}.")
            {

            }
        }

        internal enum AppointmentProperty
        {
            DOCTOR,
            PATIENT,
            SCHEDULED_FOR,
        }

        internal static void CreateAppointment(string inputCancelString, UserAccount user)
        {
            var allAppointmentProperties = Enum.GetValues(typeof(AppointmentProperty)).Cast<AppointmentProperty>().ToList();
            try
            {
                Appointment appointment = InputAppointment(inputCancelString, allAppointmentProperties, user);
                IS.Instance.AppointmentRepo.Add(appointment);
            }
            catch (InputCancelledException)
            {
            }
            catch (FailedInputAppointmentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal static void UpdateAppointment(string inputCancelString, UserAccount user)
        {
            try
            {
                List<Appointment> selectableAppointments = GetModifiableAppointments();
                if (user.Type == UserAccount.AccountType.PATIENT)
                {
                    selectableAppointments = selectableAppointments.Where(a => a.Patient.Person.Id == user.Person.Id).ToList();
                }

                Console.WriteLine(hintSelectAppointment);
                Appointment appointment = EasyInput<Appointment>.Select(selectableAppointments, inputCancelString);

                Console.WriteLine(appointment.ToString());
                Console.WriteLine(hintSelectProperties);


                List<AppointmentProperty> selectableProperties = Enum.GetValues(typeof(AppointmentProperty)).Cast<AppointmentProperty>().ToList();
                if (user.Type == UserAccount.AccountType.PATIENT)
                {
                    selectableProperties.Remove(AppointmentProperty.PATIENT);
                }

                var propertiesToUpdate = EasyInput<AppointmentProperty>.SelectMultiple(
                    selectableProperties,
                    a => Enum.GetName(typeof(AppointmentProperty), a),
                    inputCancelString
                ).ToList();

                var updatedAppointment = InputAppointment(inputCancelString, propertiesToUpdate, user, appointment);
                CopyAppointment(appointment, updatedAppointment, propertiesToUpdate);
            }
            catch (InputCancelledException)
            {
            }
            catch (NothingToSelectException)
            {
                Console.WriteLine(hintAppointmentsNotAvailable);
            }
            catch (FailedInputAppointmentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal static void DeleteAppointment(string inputCancelString, UserAccount user)
        {
            Console.WriteLine(hintSelectAppointments);

            try
            {
                List<Appointment> selectableAppointments = GetModifiableAppointments();
                if (user.Type == UserAccount.AccountType.PATIENT)
                {
                    selectableAppointments = selectableAppointments.Where(a => a.Patient.Person.Id == user.Person.Id).ToList();
                }

                var appointmentsToDelete = EasyInput<Appointment>.SelectMultiple(selectableAppointments, inputCancelString);
                foreach (Appointment appointment in appointmentsToDelete)
                {
                    IS.Instance.AppointmentRepo.Remove(appointment);
                }
            }
            catch (NothingToSelectException)
            {
                Console.WriteLine(hintAppointmentsNotAvailable);
            }
            catch (InputCancelledException)
            {
            }
        }

        internal static Appointment InputAppointment(string inputCancelString, List<AppointmentProperty> whichProperties, UserAccount user, Appointment referenceAppointment = null)
        {
            var appointment = new Appointment();

            if (whichProperties.Contains(AppointmentProperty.DOCTOR))
            {
                try
                {
                    Console.WriteLine(hintInputDoctor);
                    appointment.Doctor = InputDoctor(inputCancelString, referenceAppointment);
                }
                catch (NothingToSelectException)
                {
                    appointment.Doctor = referenceAppointment?.Doctor ?? throw new FailedInputAppointmentException(errMsgNoDoctorAvailable);
                    Console.WriteLine($"No doctors are available for the appointment. Defaulted to old doctor: {appointment.Doctor.ToString()}.");
                }
            }

            if (whichProperties.Contains(AppointmentProperty.PATIENT))
            {
                if (user.Type == UserAccount.AccountType.PATIENT)
                {
                    appointment.Patient = IS.Instance.Hospital.Patients.Find(p => p.Person.Id == user.Person.Id);
                }
                else
                {
                    try
                    {
                        Console.WriteLine(hintInputPatient);
                        appointment.Patient = InputPatient(inputCancelString, referenceAppointment);
                    }
                    catch (NothingToSelectException)
                    {
                        appointment.Patient = referenceAppointment?.Patient ?? throw new FailedInputAppointmentException(errMsgNoPatientAvailable);
                        Console.WriteLine($"No patients are available for the appointment. Defaulted to old patient: {appointment.Patient.ToString()}.");
                    }
                }
            }

            if (whichProperties.Contains(AppointmentProperty.SCHEDULED_FOR))
            {
                Console.WriteLine(hintInputScheduledFor);

                // These should not throw, but we cover the case anyway.
                appointment.Patient = appointment?.Patient ?? referenceAppointment?.Patient ?? throw new FailedInputAppointmentException(errMsgNoPatientAvailable);
                appointment.Doctor = appointment?.Doctor ?? referenceAppointment?.Doctor ?? throw new FailedInputAppointmentException(errMsgNoDoctorAvailable);

                appointment.ScheduledFor = InputScheduledFor(inputCancelString, appointment);
            }

            return appointment;
        }

        internal static Doctor InputDoctor(string inputCancelString, Appointment referenceAppointment)
        {
            return EasyInput<Doctor>.Select(GetAvailableDoctors(referenceAppointment), inputCancelString);
        }

        internal static Patient InputPatient(string inputCancelString, Appointment referenceAppointment)
        {
            return EasyInput<Patient>.Select(GetAvailablePatients(referenceAppointment), inputCancelString);
        }

        internal static DateTime InputScheduledFor(string inputCancelString, Appointment referenceAppointment)
        {
            return EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>>()
                {
                    newSchedule => newSchedule.CompareTo(DateTime.Now) > 0,
                    newSchedule => IsAvailable(referenceAppointment.Patient, referenceAppointment, newSchedule),
                    newSchedule => IsAvailable(referenceAppointment.Doctor, referenceAppointment, newSchedule)
                },
                new string[]
                {
                    hintDateTimeNotInFuture,
                    hintPatientNotAvailable,
                    hintDoctorNotAvailable
                },
                inputCancelString);
        }
        private static List<Appointment> GetModifiableAppointments()
        {
            return IS.Instance.Hospital.Appointments.Where(a => !a.Deleted && CanModifyAppointment(a.ScheduledFor)).ToList();
        }

        internal static bool CanModifyAppointment(DateTime scheduledFor)
        {
            TimeSpan difference = scheduledFor - DateTime.Now;
            return difference.TotalDays >= daysBeforeAppointmentUnmodifiable;
        }

        private static List<Doctor> GetNonDeletedDoctors()
        {
            return IS.Instance.Hospital.Doctors.Where(d => !d.Deleted).ToList();
        }

        private static List<Patient> GetNonDeletedPatients()
        {
            return IS.Instance.Hospital.Patients.Where(p => !p.Deleted).ToList();
        }

        private static List<Doctor> GetAvailableDoctors(Appointment referenceAppointment)
        {
            if (referenceAppointment == null)
            {
                return GetNonDeletedDoctors();
            }

            var availableDoctors = new List<Doctor>();
            foreach (Doctor doctor in GetNonDeletedDoctors())
            {
                if (IsAvailable(doctor, referenceAppointment))
                {
                    availableDoctors.Add(doctor);
                }
            }
            return availableDoctors;
        }

        private static List<Patient> GetAvailablePatients(Appointment referenceAppointment)
        {
            if (referenceAppointment == null)
            {
                return GetNonDeletedPatients();
            }

            var availablePatients = new List<Patient>();
            foreach (Patient patient in GetNonDeletedPatients())
            {
                if (IsAvailable(patient, referenceAppointment))
                {
                    availablePatients.Add(patient);
                }
            }
            return availablePatients;
        }


        private static bool IsAvailable(Patient patient, Appointment referenceAppointment)
        {
            return IsAvailable(patient, referenceAppointment, referenceAppointment.ScheduledFor);
        }

        private static bool IsAvailable(Patient patient, Appointment referenceAppointment, DateTime newSchedule)
        {
            foreach (Appointment appointment in GetModifiableAppointments())
            {
                if ((patient == appointment.Patient) && (appointment != referenceAppointment))
                {
                    TimeSpan timeSpan = appointment.ScheduledFor - newSchedule;
                    if (Math.Abs(timeSpan.TotalMinutes) < lengthOfAppointmentInMinutes)
                    {
                        return false;
                    }

                }
            }
            return true;
        }

        private static bool IsAvailable(Doctor doctor, Appointment referenceAppointment)
        {
            return IsAvailable(doctor, referenceAppointment, referenceAppointment.ScheduledFor);
        }

        private static bool IsAvailable(Doctor doctor, Appointment referenceAppointment, DateTime newSchedule)
        {
            foreach (Appointment appointment in GetModifiableAppointments())
            {
                if ((doctor == appointment.Doctor) && (appointment != referenceAppointment))
                {
                    TimeSpan timeSpan = appointment.ScheduledFor - newSchedule;
                    if (Math.Abs(timeSpan.TotalMinutes) < lengthOfAppointmentInMinutes)
                    {
                        return false;
                    }

                }
            }
            return true;
        }

        private static void CopyAppointment(Appointment target, Appointment source, List<AppointmentProperty> whichProperties)
        {
            if (whichProperties.Contains(AppointmentProperty.DOCTOR)) target.Doctor = source.Doctor;
            if (whichProperties.Contains(AppointmentProperty.PATIENT)) target.Patient = source.Patient;
            if (whichProperties.Contains(AppointmentProperty.SCHEDULED_FOR)) target.ScheduledFor = source.ScheduledFor;
        }
    }
}
