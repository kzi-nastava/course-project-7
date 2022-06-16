using HIS.Core.AppointmentModel;
using HIS.Core.AppointmentModel.AppointmentAvailability;
using HIS.Core.AppointmentModel.Util;
using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.DoctorModel.DoctorAvailability;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.PatientModel.PatientAvailability;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.RoomModel;
using HIS.Core.RoomModel.RoomAvailability;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.PersonModel.PatientModel.MedicalRecordModel;
using HIS.Core.PersonModel.PatientModel.MedicalRecordModel.ReferralModel;

namespace HIS.CLI.View
{
	internal class AppointmentView : AbstractView
	{
		private readonly IAppointmentService _service;
		private readonly IAppointmentAvailabilityService _appointmentAvailabilityService;
		private readonly IDoctorService _doctorService;
		private readonly IDoctorAvailabilityService _doctorAvailabilityService;
		private readonly IPatientService _patientService;
		private readonly IPatientAvailabilityService _patientAvailabilityService;
		private readonly IRoomService _roomService;
		private readonly IRoomAvailabilityService _roomAvailabilityService;
		private readonly IMedicalRecordService _medicalRecordService;
		private readonly MedicalRecordView _medicalRecordView;
		private readonly ReferralView _referralView;
		private readonly PrescriptionView _prescriptionView;
		private IEnumerable<AppointmentProperty> _properties;

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
			"Do you want to start appointment?";

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
			"Do you want to make a referral?";
		private const string hintWritePrescription =
			"Do you want to write a prescription?";
		private const string hintInputDate = "Input the date for which you want to be given scheduled appointments";

		private const string hintAnamensisUpdated = "You've successfully updated appointment's anamnesis";

		private const string hintInputAnamnesis = "Input anamnesis (newLine to finish)";

		public AppointmentView(IAppointmentService service, IAppointmentAvailabilityService appointmentAvailabilityService, IDoctorService doctorService, IDoctorAvailabilityService doctorAvailabilityService, IPatientService patientService,
			IPatientAvailabilityService patientAvailabilityService, IRoomService roomService, IRoomAvailabilityService roomAvailabilityService, IMedicalRecordService medicalRecordService, MedicalRecordView medicalRecordView, ReferralView referralView,
			PrescriptionView prescriptionView)
		{
			_service = service;
			_appointmentAvailabilityService = appointmentAvailabilityService;
			_doctorService = doctorService;
			_doctorAvailabilityService = doctorAvailabilityService;
			_patientService = patientService;
			_patientAvailabilityService = patientAvailabilityService;
			_roomService = roomService;
			_roomAvailabilityService = roomAvailabilityService;
			_medicalRecordService = medicalRecordService;
			_medicalRecordView = medicalRecordView;
			_properties = Utility.GetEnumValues<AppointmentProperty>();
			_referralView = referralView;
			_prescriptionView = prescriptionView;
		}

		internal void CreateWithPredefinedProperties(IEnumerable<AppointmentProperty> predefProperties, Appointment appointment)
		{
			var propertiesToInput = AppointmentPropertyHelpers.GetProperties().Where(p => !predefProperties.Contains(p));
			InputAppointment(propertiesToInput, appointment);
			_service.Add(appointment, User);
		}

		internal void CmdCreate()
		{
			try
			{
				CreateWithPredefinedProperties(new List<AppointmentProperty>(), new Appointment());
				Hint(hintAppointmentScheduled);
			}
			catch (NothingToSelectException e)
			{
				Error(e.Message);
			}
		}

		internal void CmdRead()
		{
			IEnumerable<Appointment> allAppointments = new List<Appointment>();
			if (User.Type == UserAccount.AccountType.PATIENT)
			{
				allAppointments = _service.GetAll(_patientService.GetPatientFromPerson(User.Person));
			}

			if (User.Type == UserAccount.AccountType.DOCTOR)
			{
				allAppointments = _service.GetAll(_doctorService.GetDoctorFromPerson(User.Person));
			}

			foreach (var app in allAppointments)
			{
				Print(app.ToString());
			}
		}

		internal void CmdUpdate()
		{
			try
			{
				Appointment appointment = SelectModifiableAppointment();
				var propertiesToUpdate = SelectModifiableProperties();
				var updatedAppointment = new Appointment();
				InputAppointment(propertiesToUpdate, updatedAppointment, appointment);
				_service.Update(appointment, updatedAppointment, propertiesToUpdate, User);
				Hint(hintAppointmentUpdated);
			}
			catch (NothingToSelectException e)
			{
				Error(e.Message);
			}
		}

		internal void CmdDelete()
		{
			try
			{
				var appointmentsToDelete = SelectModifiableAppointments();
				foreach (Appointment appointment in appointmentsToDelete)
				{
					_service.Remove(appointment, User);
					Hint(hintAppointmentDeleted);
				}
			}
			catch (NothingToSelectException e)
			{
				Error(e.Message);
			}
		}

		private IEnumerable<AppointmentProperty> SelectModifiableProperties()
		{
			Hint(hintSelectProperties);
			return EasyInput<AppointmentProperty>.SelectMultiple(
				AppointmentPropertyHelpers.GetModifiableProperties(User).ToList(),
				ap => ap.ToString(),
				_cancel
			).ToList();
		}

		private Appointment SelectModifiableAppointment()
		{
			Hint(hintSelectAppointment);
			return EasyInput<Appointment>.Select(
				_service.GetModifiable(User), _cancel);
		}

		private List<Appointment> SelectModifiableAppointments()
		{
			Hint(hintSelectAppointments);
			return EasyInput<Appointment>.SelectMultiple(
				_service.GetModifiable(User).ToList(), _cancel).ToList();
		}

		private void InputAppointment(IEnumerable<AppointmentProperty> whichProperties, Appointment appointment, Appointment refAppointment = null)
		{
			if (whichProperties.Contains(AppointmentProperty.DOCTOR))
			{
				appointment.Doctor = InputDoctor(refAppointment);
			}

			if (whichProperties.Contains(AppointmentProperty.PATIENT))
			{
				appointment.Patient = InputPatient(refAppointment);
			}

			if (whichProperties.Contains(AppointmentProperty.ROOM))
			{
				appointment.Room = InputExaminationRoom(refAppointment);
			}

			if (whichProperties.Contains(AppointmentProperty.SCHEDULED_FOR))
			{
				appointment.ScheduledFor = InputScheduledFor(appointment, refAppointment);
			}
		}

		private Doctor InputDoctor(Appointment referenceAppointment)
		{
			if (User.Type == UserAccount.AccountType.DOCTOR)
			{
				return _doctorService.GetDoctorFromPerson(User.Person);
			}
			else
			{
				Hint(hintSelectDoctor);
				return EasyInput<Doctor>.Select(_doctorAvailabilityService.GetAvailable(referenceAppointment), _cancel);
			}
		}


		private Patient InputPatient(Appointment referenceAppointment)
		{
			if (User.Type == UserAccount.AccountType.PATIENT)
			{
				return _patientService.GetPatientFromPerson(User.Person);
			}
			else
			{
				Hint(hintSelectPatient);
				return EasyInput<Patient>.Select(_patientAvailabilityService.GetAvailable(referenceAppointment), _cancel);
			}
		}

		private Room InputExaminationRoom(Appointment referenceAppointment)
		{
			// Patient and doctor cannot modify the Room property, however when creating an Appointment we can reach here.
			if (User.Type != UserAccount.AccountType.MANAGER)
			{
				return _roomAvailabilityService.GetRandomAvailableExaminationRoom(referenceAppointment);
			}
			else
			{
				Hint(hintSelectExaminationRoom);
				return EasyInput<Room>.Select(_roomAvailabilityService.GetAvailableExaminationRooms(referenceAppointment), _cancel);
			}
		}

		private DateTime InputScheduledFor(Appointment proposedAppointment, Appointment referenceAppointment)
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

			Hint(hintGetScheduledFor);
			return EasyInput<DateTime>.Get(
				new List<Func<DateTime, bool>>()
				{
					newSchedule => newSchedule.CompareTo(DateTime.Now) > 0,
					newSchedule => _patientAvailabilityService.IsAvailable(patient, newSchedule, patientReferenceAppointment),
					newSchedule => _doctorAvailabilityService.IsAvailable(doctor, newSchedule, doctorReferenceAppointment),
					newSchedule => _roomAvailabilityService.IsAvailable(room, newSchedule, roomReferenceAppointment),
				},
				new string[]
				{
					hintDateTimeNotInFuture,
					hintPatientNotAvailable,
					hintDoctorNotAvailable,
					hintExaminationRoomNotAvailable,
				},
				_cancel);
		}

		internal void CmdCreateRecommended()
		{
			try
			{
				AppointmentSearchBundle sb = InputSearchBundle();
				AppointmentProperty priority = InputPrioritizedProperty();

				Appointment appointment = GetRecommendedAppointment(sb, priority);
				Print(appointment.ToString());
				Hint(askCreateAppointment);
				if (EasyInput<bool>.YesNo(_cancel))
				{
					_service.Add(appointment, User);
				}
			}
			catch (NothingToSelectException e)
			{
				Error(e.Message);
			}
		}

		private AppointmentSearchBundle InputSearchBundle()
		{
			Doctor doctor = InputDoctor(null);
			Patient patient = InputPatient(null);
			TimeSpan start = InputStartOfRange();
			TimeSpan end = InputEndOfRange(start);
			DateTime latestDate = InputLatestDate();

			return new AppointmentSearchBundle(doctor, patient, start, end, latestDate);
		}

		private Appointment GetRecommendedAppointment(AppointmentSearchBundle sb, AppointmentProperty priority)
		{
			return GetOptimalAppointment(sb) ?? GetPrioritizedAppointment(sb, priority) ?? GetDesperateAppointment(sb);
		}

		private Appointment GetOptimalAppointment(AppointmentSearchBundle sb)
		{
			return _appointmentAvailabilityService.FindRecommendedAppointment(sb);
		}
		private Appointment GetPrioritizedAppointment(AppointmentSearchBundle sb, AppointmentProperty priority)
		{
			Hint(hintOptimalSearchFailed);

			AppointmentSearchBundle sbPrioritized;
			if (priority == AppointmentProperty.DOCTOR)
			{
				sbPrioritized = AppointmentSearchBundle.IgnoreInterval(sb);
			}
			else
			{
				sbPrioritized = AppointmentSearchBundle.IgnoreDoctor(sb);
			}
			return _appointmentAvailabilityService.FindRecommendedAppointment(sbPrioritized);
		}

		private Appointment GetDesperateAppointment(AppointmentSearchBundle sb)
		{
			Hint(hintPrioritySearchFailed);

			var desperateAppointments = new List<Appointment>();
			var desperateAppointment = new Appointment();

			void processDesperate(Func<AppointmentSearchBundle, AppointmentSearchBundle> newSb, string hint)
			{
				desperateAppointment = _appointmentAvailabilityService.FindRecommendedAppointment(newSb(sb));
				if (desperateAppointment != null)
				{
					Console.ForegroundColor = ConsoleColor.Cyan;
					Print(hint);
					Console.ForegroundColor = ConsoleColor.Gray;
					Print(desperateAppointment.ToString());
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

			Hint(hintSelectAppointment);
			return EasyInput<Appointment>.Select(desperateAppointments, _cancel);
		}

		private TimeSpan InputStartOfRange()
		{
			Hint(hintGetStartOfRange);
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
				_cancel,
				TimeSpan.Parse);
		}

		private TimeSpan InputEndOfRange(TimeSpan start)
		{
			Hint(hintGetEndOfRange);
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
				_cancel,
				TimeSpan.Parse);
		}

		private DateTime InputLatestDate()
		{
			Hint(hintGetLatestDesiredDate);
			return EasyInput<DateTime>.Get(
				new List<Func<DateTime, bool>>()
				{
					dt => AppointmentSearchBundle.DtNotTooSoon(dt),
				},
				new string[]
				{
					AppointmentSearchBundle.ErrDateTooSoon,
				},
				_cancel);
		}

		private AppointmentProperty InputPrioritizedProperty()
		{
			Hint(hintGetPrioritizedProperty);
			return EasyInput<AppointmentProperty>.Select(AppointmentPropertyHelpers.GetPrioritizableProperties(), _cancel);
		}
		
		internal void CmdViewAndStartAppointments()
		{
			DateTime firstRelevantDay = GetFirstRelevantDayOfAppointments();
			List <Appointment> nextAppointments = _service.GetNextDoctorsAppointments(User, firstRelevantDay);
			PrintAppointmentsAndMedicalRecords(nextAppointments);
			if (nextAppointments.Count == 0) return;
            
			Hint(hintCheckStartingAppointment);
			if (EasyInput<bool>.YesNo(_cancel)) //wants to start appointment
			{
				StartAppointment(nextAppointments);
			}
		}
		
		private DateTime GetFirstRelevantDayOfAppointments()
		{
			Hint(hintInputDate);
            
			DateTime firstRelevantDay = EasyInput<DateTime>.Get(
				new List<Func<DateTime, bool>>()
				{
					newSchedule => newSchedule.CompareTo(DateTime.Now) > 0,
				},
				new string[]
				{
					hintDateTimeNotInFuture,
				},
				_cancel);
            
			return firstRelevantDay;
		}
		
		private void PrintAppointmentsAndMedicalRecords(List<Appointment> appointments)
		{
			if (appointments.Count == 0)
			{
				Hint(hintNoScheduledAppoinments);
			}
			else
			{
				foreach (var appointment in appointments)
				{
					Console.WriteLine(appointment.ToString());
					MedicalRecord medicalRecord = _medicalRecordService.GetPatientsMedicalRecord(appointment.Patient);
					Print(medicalRecord.ToString());
					Console.WriteLine("=====================================");
				}
			}
		}
		
		private void StartAppointment(List<Appointment> startableAppointments)
		{
			Hint(hintSelectAppointment);
			var appointmentToStart = EasyInput<Appointment>.Select(startableAppointments, _cancel);
            
			_medicalRecordView.UpdateMedicalRecord(appointmentToStart.Patient);
			UpdateAnamnesis(appointmentToStart);
			
			Hint(hintMakeReferral);
			if (EasyInput<bool>.YesNo(_cancel)) //wants to create a referral
			{
				_referralView.CreateReferral(appointmentToStart);
			}
            
			
			Hint(hintWritePrescription);
			if (EasyInput<bool>.YesNo(_cancel)) //wants to create a prescription
			{
				_prescriptionView.CreatePrescription(_medicalRecordService.GetPatientsMedicalRecord(appointmentToStart.Patient));
			}
            
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(hintAppointmentIsOver);
			Console.ForegroundColor = ConsoleColor.Gray;
            
			/*
			EquipmentModel.DeleteEquipmentAfterAppointment(appointmentToStart.Room);
			*/
		}
		
		private void UpdateAnamnesis(Appointment appointment)
		{
			Appointment updatedAppointment = appointment;
			updatedAppointment.Anamnesis = InputAnamnesis();
			_service.Copy(appointment, updatedAppointment, _properties);
			Print(hintAnamensisUpdated);
		}
        
		private string InputAnamnesis()
		{
			Hint(hintInputAnamnesis);
			return Console.ReadLine();
		}
		
    internal void CmdCreateUrgent()
		{
			throw new NotImplementedException();
		}
    
    internal void Print(List<Appointment> appointments)
    {
	    foreach (var appointment in appointments)
	    {
		    Print(appointment.ToString());
	    }
    }

	}
}
