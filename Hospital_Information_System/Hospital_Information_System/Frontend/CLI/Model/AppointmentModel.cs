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
        private const string hintDoctorsNotAvailable = "No valid doctor available";
        private const string hintPatientsNotAvailable = "No valid patient available";

        private const int lengthOfAppointmentInMinutes = 15;
        private const int daysBeforeAppointmentUnmodifiable = 1;
        private const int daysBeforeModificationNeedsRequest = 2;

        internal static void CreateAppointment(string inputCancelString, UserAccount user)
        {
            List<AppointmentProperty> allAppointmentProperties = GetAllAppointmentProperties();
            try
            {
                Appointment appointment = InputAppointment(inputCancelString, allAppointmentProperties, user, null);
                IS.Instance.UserAccountRepo.AddCreatedAppointmentTimestamp(user, DateTime.Now);
                IS.Instance.AppointmentRepo.Add(appointment);
            }
            catch (InputCancelledException)
            {
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal static void UpdateAppointment(string inputCancelString, UserAccount user)
        {
            try
            {
                List<Appointment> modifiableAppointments = GetModifiableAppointments(user);

                Console.WriteLine(hintSelectAppointment);
                Appointment appointment = EasyInput<Appointment>.Select(modifiableAppointments, inputCancelString);

                Console.WriteLine(appointment.ToString());
                Console.WriteLine(hintSelectProperties);

                List<AppointmentProperty> modifiableProperties = GetModifiableProperties(user);

                var propertiesToUpdate = EasyInput<AppointmentProperty>.SelectMultiple(
                    modifiableProperties,
                    a => Enum.GetName(typeof(AppointmentProperty), a),
                    inputCancelString
                ).ToList();

                var updatedAppointment = InputAppointment(inputCancelString, propertiesToUpdate, user, appointment);

                IS.Instance.UserAccountRepo.AddModifiedAppointmentTimestamp(user, DateTime.Now);

                if (MustRequest(appointment.ScheduledFor, user))
                {
                    var proposedAppointment = new Appointment();
                    CopyAppointment(proposedAppointment, appointment, GetAllAppointmentProperties());
                    CopyAppointment(proposedAppointment, updatedAppointment, propertiesToUpdate);
                    IS.Instance.UpdateRequestRepo.Add(new UpdateRequest(user, appointment, proposedAppointment));
                }
                else
                {
                    CopyAppointment(appointment, updatedAppointment, propertiesToUpdate);
                }
            }
            catch (InputCancelledException)
            {
            }
            // Potentially thrown by the attempted selection of appointment early in this function
            catch (NothingToSelectException)
            {
                throw new InputFailedException(hintAppointmentsNotAvailable);
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal static void DeleteAppointment(string inputCancelString, UserAccount user)
        {
            Console.WriteLine(hintSelectAppointments);

            try
            {
                List<Appointment> modifiableAppointments = GetModifiableAppointments(user);

                var appointmentsToDelete = EasyInput<Appointment>.SelectMultiple(modifiableAppointments, inputCancelString);
                foreach (Appointment appointment in appointmentsToDelete)
                {
                    IS.Instance.UserAccountRepo.AddModifiedAppointmentTimestamp(user, DateTime.Now);

                    if (MustRequest(appointment.ScheduledFor, user))
                    {
                        IS.Instance.DeleteRequestRepo.Add(new DeleteRequest(user, appointment));
                    }
                    else
                    {
                        IS.Instance.AppointmentRepo.Remove(appointment);
                    }
                }
            }
            catch (InputCancelledException)
            {
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static Appointment InputAppointment(string inputCancelString, List<AppointmentProperty> whichProperties, UserAccount user, Appointment refAppointment)
        {
            var appointment = new Appointment();

            if (whichProperties.Contains(AppointmentProperty.DOCTOR))
            {
                Console.WriteLine(hintInputDoctor);
                appointment.Doctor = InputDoctor(inputCancelString, refAppointment);
            }

            if (whichProperties.Contains(AppointmentProperty.PATIENT))
            {
                if (user.Type == UserAccount.AccountType.PATIENT)
                {
                    appointment.Patient = IS.Instance.Hospital.Patients.Find(p => p.Person.Id == user.Person.Id);
                }
                else
                {
                    Console.WriteLine(hintInputPatient);
                    appointment.Patient = InputPatient(inputCancelString, refAppointment);
                }
            }

            if (whichProperties.Contains(AppointmentProperty.SCHEDULED_FOR))
            {
                Console.WriteLine(hintInputScheduledFor);

                appointment.ScheduledFor = InputScheduledFor(inputCancelString, appointment, refAppointment);
            }

            return appointment;
        }

        private static Doctor InputDoctor(string inputCancelString, Appointment referenceAppointment)
        {
            try
            {
                return EasyInput<Doctor>.Select(GetAvailableDoctors(referenceAppointment), inputCancelString);
            }
            catch (NothingToSelectException)
            {
                throw new InputFailedException(hintDoctorsNotAvailable);
            }
        }

        private static Patient InputPatient(string inputCancelString, Appointment referenceAppointment)
        {
            try
            {
                return EasyInput<Patient>.Select(GetAvailablePatients(referenceAppointment), inputCancelString);
            }
            catch (NothingToSelectException)
            {
                throw new InputFailedException(hintPatientsNotAvailable);
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

            return EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>>()
                {
                    newSchedule => newSchedule.CompareTo(DateTime.Now) > 0,
                    newSchedule => IsAvailable(patient, patientReferenceAppointment, newSchedule),
                    newSchedule => IsAvailable(doctor, doctorReferenceAppointment, newSchedule)
                },
                new string[]
                {
                    hintDateTimeNotInFuture,
                    hintPatientNotAvailable,
                    hintDoctorNotAvailable
                },
                inputCancelString);
        }

        private enum AppointmentProperty
        {
            DOCTOR,
            PATIENT,
            SCHEDULED_FOR,
        }

        private static List<AppointmentProperty> GetModifiableProperties(UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                List<AppointmentProperty> modifiableProperties = GetAllAppointmentProperties();
                modifiableProperties.Remove(AppointmentProperty.PATIENT);
                return modifiableProperties;
            }
            else
            {
                return GetAllAppointmentProperties();
            }
        }

        private static List<AppointmentProperty> GetAllAppointmentProperties()
        {
            return Enum.GetValues(typeof(AppointmentProperty)).Cast<AppointmentProperty>().ToList();
        }

        private static List<Appointment> GetModifiableAppointments(UserAccount user)
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

        private static List<Appointment> GetModifiableAppointments()
        {
            return IS.Instance.Hospital.Appointments.Where(a => !a.Deleted).ToList();
        }

        private static bool CanModifyAppointment(DateTime scheduledFor, UserAccount user)
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

        private static bool MustRequest(DateTime scheduledFor, UserAccount user)
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

        private static List<Doctor> GetModifiableDoctors()
        {
            return IS.Instance.Hospital.Doctors.Where(d => !d.Deleted).ToList();
        }

        private static List<Patient> GetModifiablePatients()
        {
            return IS.Instance.Hospital.Patients.Where(p => !p.Deleted).ToList();
        }

        private static List<Doctor> GetAvailableDoctors(Appointment refAppointment)
        {
            if (refAppointment == null)
            {
                return GetModifiableDoctors();
            }

            return GetModifiableDoctors().Where(d => IsAvailable(d, refAppointment, refAppointment.ScheduledFor)).ToList();
        }

        private static List<Patient> GetAvailablePatients(Appointment refAppointment)
        {
            if (refAppointment == null)
            {
                return GetModifiablePatients();
            }

            return GetModifiablePatients().Where(p => IsAvailable(p, refAppointment, refAppointment.ScheduledFor)).ToList();
        }

        private static bool IsAvailable(Patient patient, Appointment refAppointment, DateTime newSchedule)
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

        private static bool IsAvailable(Doctor doctor, Appointment refAppointment, DateTime newSchedule)
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

        private static bool AreColliding(DateTime schedule1, DateTime schedule2)
        {
            TimeSpan difference = schedule1 - schedule2;
            return Math.Abs(difference.TotalMinutes) < lengthOfAppointmentInMinutes;
        }

        private static void CopyAppointment(Appointment target, Appointment source, List<AppointmentProperty> whichProperties)
        {
            if (whichProperties.Contains(AppointmentProperty.DOCTOR)) target.Doctor = source.Doctor;
            if (whichProperties.Contains(AppointmentProperty.PATIENT)) target.Patient = source.Patient;
            if (whichProperties.Contains(AppointmentProperty.SCHEDULED_FOR)) target.ScheduledFor = source.ScheduledFor;
        }
    }
}
