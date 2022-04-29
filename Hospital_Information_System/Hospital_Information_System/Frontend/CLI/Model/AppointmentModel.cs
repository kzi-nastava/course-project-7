using HospitalIS.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class AppointmentModel
    {
        private const string hintSelectAppointments = "Select appointments by their number, separated by whitespace.\nEnter a newline to finish";
        private const string hintSelectAppointment = "Select appointment";
        private const string hintSelectProperties = "Select properties by their number, separated by whitespace.\nEnter a newline to finish";
        private const string hintInputDoctor = "Select doctor for the appointment";
        private const string hintInputPatient = "Select patient for the appointment";
        private const string hintInputExaminationRoom = "Select examination room for the appointment";
        private const string hintInputScheduledFor = "Enter date and time for the appointment";
        private const string hintRequestSent = "Request for modification has been sent";
        private const string hintPatientNotAvailable = "Patient is not available at the selected date and time";
        private const string hintDoctorNotAvailable = "Doctor is not available at the selected date and time";
        private const string hintExaminationRoomNotAvailable = "Examination room is not available at the selected date and time";
        private const string hintDateTimeNotInFuture = "Date and time must be in the future";

        internal static void CreateAppointment(string inputCancelString, UserAccount user)
        {
            var allAppointmentProperties = AppointmentController.GetAllAppointmentProperties();
            try
            {
                Appointment appointment = InputAppointment(inputCancelString, allAppointmentProperties, user);
                UserAccountController.AddCreatedAppointmentTimestamp(user, DateTime.Now);
                IS.Instance.AppointmentRepo.Add(appointment);
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal static void UpdateAppointment(string inputCancelString, UserAccount user)
        {
            Console.WriteLine(hintSelectAppointment);
            try
            {
                Appointment appointment = SelectModifiableAppointment(inputCancelString, user);

                Console.WriteLine(appointment.ToString());
                Console.WriteLine(hintSelectProperties);

                var propertiesToUpdate = SelectModifiableProperties(inputCancelString, user);

                Appointment updatedAppointment = InputAppointment(inputCancelString, propertiesToUpdate, user, appointment);

                UserAccountController.AddModifiedAppointmentTimestamp(user, DateTime.Now);

                if (AppointmentController.MustRequestAppointmentModification(appointment.ScheduledFor, user))
                {
                    var proposedAppointment = new Appointment();
                    AppointmentController.CopyAppointment(proposedAppointment, appointment, AppointmentController.GetAllAppointmentProperties());
                    AppointmentController.CopyAppointment(proposedAppointment, updatedAppointment, propertiesToUpdate);
                    IS.Instance.UpdateRequestRepo.Add(new UpdateRequest(user, appointment, proposedAppointment));
                    Console.WriteLine(hintRequestSent);
                }
                else
                {
                    AppointmentController.CopyAppointment(appointment, updatedAppointment, propertiesToUpdate);
                }
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
                var appointmentsToDelete = SelectModifiableAppointments(inputCancelString, user);
                foreach (Appointment appointment in appointmentsToDelete)
                {
                    UserAccountController.AddModifiedAppointmentTimestamp(user, DateTime.Now);

                    if (AppointmentController.MustRequestAppointmentModification(appointment.ScheduledFor, user))
                    {
                        IS.Instance.DeleteRequestRepo.Add(new DeleteRequest(user, appointment));
                        Console.WriteLine(hintRequestSent);
                    }
                    else
                    {
                        IS.Instance.AppointmentRepo.Remove(appointment);
                    }
                }
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static List<AppointmentController.AppointmentProperty> SelectModifiableProperties(string inputCancelString, UserAccount user)
        {
            return EasyInput<AppointmentController.AppointmentProperty>.SelectMultiple(
                AppointmentController.GetModifiableProperties(user),
                ap => AppointmentController.GetAppointmentPropertyName(ap),
                inputCancelString
            ).ToList();
        }

        private static Appointment SelectModifiableAppointment(string inputCancelString, UserAccount user)
        {
            return EasyInput<Appointment>.Select(
                AppointmentController.GetModifiableAppointments(user), inputCancelString);
        }

        private static List<Appointment> SelectModifiableAppointments(string inputCancelString, UserAccount user)
        {
            return EasyInput<Appointment>.SelectMultiple(
                AppointmentController.GetModifiableAppointments(user), inputCancelString).ToList();
        }

        private static Appointment InputAppointment(string inputCancelString, List<AppointmentController.AppointmentProperty> whichProperties, UserAccount user, Appointment refAppointment = null)
        {
            var appointment = new Appointment();

            if (whichProperties.Contains(AppointmentController.AppointmentProperty.DOCTOR))
            {
                Console.WriteLine(hintInputDoctor);
                appointment.Doctor = InputDoctor(inputCancelString, refAppointment);
            }

            if (whichProperties.Contains(AppointmentController.AppointmentProperty.PATIENT))
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

            if (whichProperties.Contains(AppointmentController.AppointmentProperty.ROOM))
            {
                // Patient cannot modify the Room property, however when creating an Appointment we can reach here.
                if (user.Type == UserAccount.AccountType.PATIENT)
                {
                    // Assign a random available Room with type Examination to the Appointment.
                    var rnd = new Random();
                    var rooms = AppointmentController.GetAvailableExaminationRooms(refAppointment);
                    appointment.Room = rooms[rnd.Next(rooms.Capacity)];
                }
                else
                {
                    Console.WriteLine(hintInputExaminationRoom);
                    appointment.Room = InputExaminationRoom(inputCancelString, refAppointment);
                }
            }

            if (whichProperties.Contains(AppointmentController.AppointmentProperty.SCHEDULED_FOR))
            {
                Console.WriteLine(hintInputScheduledFor);
                appointment.ScheduledFor = InputScheduledFor(inputCancelString, appointment, refAppointment);
            }

            return appointment;
        }

        private static Doctor InputDoctor(string inputCancelString, Appointment referenceAppointment)
        {
            return EasyInput<Doctor>.Select(AppointmentController.GetAvailableDoctors(referenceAppointment), inputCancelString);
        }

        private static Patient InputPatient(string inputCancelString, Appointment referenceAppointment)
        {
            return EasyInput<Patient>.Select(AppointmentController.GetAvailablePatients(referenceAppointment), inputCancelString);
        }

        private static Room InputExaminationRoom(string inputCancelString, Appointment referenceAppointment)
        {
            return EasyInput<Room>.Select(AppointmentController.GetAvailableExaminationRooms(referenceAppointment), inputCancelString);
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
    }
}
