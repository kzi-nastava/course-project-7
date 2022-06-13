using System;
using System.IO;
using HIS.Core.Foundation;
using HIS.Core.RoomModel;
using HIS.Core.EquipmentModel;
using HIS.CLI.View;
using Newtonsoft.Json;
using HIS.Core.EquipmentModel.EquipmentRelocationModel;
using HIS.Core.RoomModel.RenovationModel;
using HIS.Core.RoomModel.RoomAvailability;
using HIS.Core.MedicationModel.IngredientModel;
using HIS.Core.MedicationModel;
using HIS.Core.MedicationModel.MedicationRequestModel;
using HIS.Core.AppointmentModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.ModificationRequestModel.DeleteRequestModel;
using HIS.Core.ModificationRequestModel.UpdateRequestModel;
using HIS.Core.PersonModel.PatientModel.MedicalRecordModel;
using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.DoctorModel.DoctorAvailability;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.PatientModel.PatientAvailability;
using HIS.Core.PersonModel;
using HIS.Core.MedicationModel.PrescriptionModel;

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

			IRoomService roomService = new RoomService(roomRepo);
			IEquipmentService equipmentService = new EquipmentService(equipmentRepo, roomService);
			IEquipmentRelocationService equipmentRelocationService = new EquipmentRelocationService(relocationRepo, roomService, _tasks);
			IRenovationService renovationService = new RenovationService(renovationRepo, _tasks, roomService);
			// Forward declare.
			IAppointmentService appointmentService = new AppointmentService();
			IRoomAvailabilityService roomAvailabilityService = new RoomAvailabilityService(roomService, renovationService, appointmentService);
			IIngredientService ingredientService = new IngredientService(ingredientRepo);
			IMedicationService medicationService = new MedicationService(medicationRepo);
			IMedicationRequestService medicationRequestService = new MedicationRequestService(medicationRequestRepo);
			IUserAccountService userAccountService = new UserAccountService(userAccountRepo);
			IDeleteRequestService deleteRequestService = new DeleteRequestService(deleteRequestRepo);
			IUpdateRequestService updateRequestService = new UpdateRequestService(updateRequestRepo);
			IMedicalRecordService medicalRecordService = new MedicalRecordService(medicalRecordRepo);
			IDoctorService doctorService = new DoctorService(doctorRepo);
			IDoctorAvailabilityService doctorAvailabilityService = new DoctorAvailabilityService(doctorService, appointmentService);
			IPatientService patientService = new PatientService(patientRepo); ;
			IPatientAvailabilityService patientAvailabilityService = new PatientAvailabilityService(patientService, appointmentService);

			// Now we have everything we need to fully create appointmentService.
			appointmentService.Copy(new AppointmentService(appointmentRepo, userAccountService, deleteRequestService, updateRequestService,
				medicalRecordService, roomAvailabilityService, doctorAvailabilityService, patientAvailabilityService));

			RoomView roomView = new RoomView(roomService, null);
			EquipmentView equipmentView = new EquipmentView(equipmentService, null);
			EquipmentRelocationView equipmentRelocationView = new EquipmentRelocationView(equipmentRelocationService, roomService, null);
			RenovationView renovationView = new RenovationView(renovationService, roomService, roomAvailabilityService, roomView, null);
			IngredientView ingredientView = new IngredientView(ingredientService, medicationService, medicationRequestService, null);
			MedicationView medicationView = new MedicationView(medicationService, ingredientService, medicationRequestService, null);
			AppointmentView appointmentView = new AppointmentView(appointmentService, doctorService, doctorAvailabilityService,
				patientService, patientAvailabilityService, roomService, roomAvailabilityService, null);

			try
			{

				medicationView.CmdUpdateRequest();
				// medicationView.CmdCreateAndSendForReview();

				//ingredientView.CmdRead();
				//ingredientView.CmdAdd();
				//ingredientView.CmdUpdate();
				// ingredientView.CmdDelete();
				//ingredientView.CmdRead();
			}
			catch (InputCancelledException)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("^C");
				Console.ResetColor();
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
		}
	}
}
