using HIS.CLI.View;
using HIS.CLI.View.UserCommand;
using HIS.Core.AppointmentModel;
using HIS.Core.AppointmentModel.AppointmentAvailability;
using HIS.Core.EquipmentModel;
using HIS.Core.EquipmentModel.EquipmentRelocationModel;
using HIS.Core.Foundation;
using HIS.Core.MedicationModel;
using HIS.Core.MedicationModel.IngredientModel;
using HIS.Core.MedicationModel.MedicationRequestModel;
using HIS.Core.MedicationModel.PrescriptionModel;
using HIS.Core.ModificationRequestModel.DeleteRequestModel;
using HIS.Core.ModificationRequestModel.UpdateRequestModel;
using HIS.Core.PersonModel;
using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.DoctorModel.DoctorAvailability;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.PatientModel.MedicalRecordModel;
using HIS.Core.PersonModel.PatientModel.PatientAvailability;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.PersonModel.UserAccountModel.Util;
using HIS.Core.PollModel.AppointmentPollModel;
using HIS.Core.PollModel.HospitalPollModel;
using HIS.Core.RoomModel;
using HIS.Core.RoomModel.RenovationModel;
using HIS.Core.RoomModel.RoomAvailability;
using Newtonsoft.Json;
using System;
using System.IO;
using HIS.Core.EquipmentModel.EquipmentRequestModel;

namespace HIS.CLI
{
	static class Startup
	{
		private static readonly string dataDir = Path.Combine("..", "..", "..", "data") + Path.DirectorySeparatorChar;
		private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None };
		private static readonly TaskQueue _tasks = new TaskQueue();

		static void Main()
		{
			IEquipmentRepository equipmentRepo = new EquipmentJSONRepository(dataDir + "db_equipment.json", jsonSettings);
			IRoomRepository roomRepo = new RoomJSONRepository(dataDir + "db_rooms.json", jsonSettings);
			IEquipmentRelocationRepository relocationRepo = new EquipmentRelocationJSONRepository(dataDir + "db_relocations.json", jsonSettings);
			IRenovationRepository renovationRepo = new RenovationJSONRepository(dataDir + "db_renovations.json", jsonSettings);
			IIngredientRepository ingredientRepo = new IngredientJSONRepository(dataDir + "db_ingredients.json", jsonSettings);
			IMedicationRepository medicationRepo = new MedicationJSONRepository(dataDir + "db_medications.json", jsonSettings);
			IMedicationRequestRepository medicationRequestRepo = new MedicationRequestJSONRepository(dataDir + "db_medication_requests.json", jsonSettings);
			IPersonRepository personRepo = new PersonJSONRepository(dataDir + "db_persons.json", jsonSettings);
			IUserAccountRepository userAccountRepo = new UserAccountJSONRepository(dataDir + "db_user_accounts.json", jsonSettings);
			IPatientRepository patientRepo = new PatientJSONRepository(dataDir + "db_patients.json", jsonSettings);
			IPrescriptionRepository prescriptionRepo = new PrescriptionJSONRepository(dataDir + "db_prescriptions.json", jsonSettings);
			IDoctorRepository doctorRepo = new DoctorJSONRepository(dataDir + "db_doctors.json", jsonSettings);
			IAppointmentRepository appointmentRepo = new AppointmentJSONRepository(dataDir + "db_appointments.json", jsonSettings);
			IMedicalRecordRepository medicalRecordRepo = new MedicalRecordJSONRepository(dataDir + "db_medical_records.json", jsonSettings);
			IDeleteRequestRepository deleteRequestRepo = new DeleteRequestJSONRepository(dataDir + "db_delete_requests.json", jsonSettings);
			IUpdateRequestRepository updateRequestRepo = new UpdateRequestJSONRepository(dataDir + "db_update_requests.json", jsonSettings);
			IHospitalPollRepository hospitalPollRepo = new HospitalPollJSONRepository(dataDir + "db_hospital_polls.json", jsonSettings);
			IAppointmentPollRepository appointmentPollRepo = new AppointmentPollJSONRepository(dataDir + "db_appointment_polls.json", jsonSettings);
			IEquipmentRequestRepository equipmentRequestRepo = new EquipmentRequestJSONRepository(dataDir + "db_equipment_requests.json", jsonSettings);

			IRoomService roomService = new RoomService(roomRepo);
			IEquipmentService equipmentService = new EquipmentService(equipmentRepo, roomService);
			IEquipmentRelocationService equipmentRelocationService = new EquipmentRelocationService(relocationRepo, roomService, _tasks);
			IRenovationService renovationService = new RenovationService(renovationRepo, _tasks, roomService);
			IIngredientService ingredientService = new IngredientService(ingredientRepo);
			IMedicationService medicationService = new MedicationService(medicationRepo);
			IMedicationRequestService medicationRequestService = new MedicationRequestService(medicationRequestRepo);
			IPatientService patientService = new PatientService(patientRepo);
			IPersonService personService = new PersonService(personRepo);
			IMedicalRecordService medicalRecordService = new MedicalRecordService(medicalRecordRepo, patientService);
			IUserAccountService userAccountService = new UserAccountService(userAccountRepo, medicalRecordService, patientService, personService);
			IDeleteRequestService deleteRequestService = new DeleteRequestService(deleteRequestRepo);
			IUpdateRequestService updateRequestService = new UpdateRequestService(updateRequestRepo);
			IHospitalPollService hospitalPollService = new HospitalPollService(hospitalPollRepo);
			IAppointmentPollService appointmentPollService = new AppointmentPollService(appointmentPollRepo);
			IDoctorService doctorService = new DoctorService(doctorRepo, appointmentPollService);
			IAppointmentService appointmentService = new AppointmentService(appointmentRepo, userAccountService, deleteRequestService, updateRequestService, medicalRecordService, appointmentPollService, doctorService);
			IDoctorAvailabilityService doctorAvailabilityService = new DoctorAvailabilityService(doctorService, appointmentService);
			IRoomAvailabilityService roomAvailabilityService = new RoomAvailabilityService(roomService, renovationService, appointmentService);
			IPatientAvailabilityService patientAvailabilityService = new PatientAvailabilityService(patientService, appointmentService);
			IAppointmentAvailabilityService appointmentAvailabilityService = new AppointmentAvailabilityService(roomAvailabilityService, doctorAvailabilityService, patientAvailabilityService);
			IEquipmentRequestService equipmentRequestService = new EquipmentRequestService(equipmentRequestRepo, roomService, _tasks);

			UserAccountView userAccountView = new UserAccountView(userAccountService);
			RoomView roomView = new RoomView(roomService);
			EquipmentView equipmentView = new EquipmentView(equipmentService, equipmentRequestService);
			EquipmentRelocationView equipmentRelocationView = new EquipmentRelocationView(equipmentRelocationService, roomService);
			RenovationView renovationView = new RenovationView(renovationService, roomService, roomAvailabilityService, roomView);
			IngredientView ingredientView = new IngredientView(ingredientService, medicationService, medicationRequestService);
			MedicationView medicationView = new MedicationView(medicationService, ingredientService, medicationRequestService);
			MedicalRecordView medicalRecordView = new MedicalRecordView(medicalRecordService, patientService, appointmentService, ingredientService);
			AppointmentView appointmentView = new AppointmentView(appointmentService, appointmentAvailabilityService, doctorService, doctorAvailabilityService, patientService, patientAvailabilityService, roomService, roomAvailabilityService, medicalRecordService, medicalRecordView);
			DoctorView doctorView = new DoctorView(doctorService, appointmentView);
			PollView pollView = new PollView();
			AppointmentPollView appointmentPollView = new AppointmentPollView(appointmentPollService, patientService, appointmentService, pollView);
			HospitalPollView hospitalPollView = new HospitalPollView(hospitalPollService, patientService, pollView);
			PollSummaryView pollSummaryView = new PollSummaryView(hospitalPollService, appointmentPollService);
			RequestView requestView = new RequestView(deleteRequestService, updateRequestService, appointmentService);

			Console.WriteLine("Type help for a list of commands");

			UserCommandView cmdView = new LoggedOutCommandView(userAccountView);

			while (true)
			{
				try
				{
					cmdView.PollCommand();
				}
				catch (InputCancelledException)
				{
					Console.WriteLine("Cancelled");
				}
				catch (UserAccountForcefullyBlockedException e)
				{
					Console.WriteLine(e.Message);
				}
				catch (UserAccountChangedException)
				{
					cmdView = AbstractView.User.Type switch
					{
						UserAccount.AccountType.MANAGER => new ManagerCommandView(roomView, equipmentView, equipmentRelocationView, renovationView, ingredientView, medicationView, pollSummaryView),
						UserAccount.AccountType.PATIENT => new PatientCommandView(appointmentView, medicalRecordView, doctorView, hospitalPollView, appointmentPollView),
						UserAccount.AccountType.SECRETARY => new SecretaryCommandView(userAccountView, appointmentView, medicalRecordView, requestView, equipmentView),
						UserAccount.AccountType.DOCTOR => new DoctorCommandView(appointmentView),
						UserAccount.AccountType.LOGGED_OUT => new LoggedOutCommandView(userAccountView),
						_ => throw new NotImplementedException(),
					};
				}
				catch (QuitApplicationException)
				{
					break;
				}
			}

			Console.WriteLine("Press any key to exit...");
			Console.ReadLine();

			roomRepo.Save();
			equipmentRepo.Save();
			relocationRepo.Save();
			renovationRepo.Save();
			ingredientRepo.Save();
			medicationRepo.Save();
			medicationRequestRepo.Save();
			personRepo.Save();
			userAccountRepo.Save();
			patientRepo.Save(); ;
			prescriptionRepo.Save();
			doctorRepo.Save();
			appointmentRepo.Save();
			medicalRecordRepo.Save();
			deleteRequestRepo.Save();
			updateRequestRepo.Save();
			hospitalPollRepo.Save();
			appointmentPollRepo.Save();
			equipmentRequestRepo.Save();
		}
	}
}
