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

			IRoomService roomService = new RoomService(roomRepo);
			IEquipmentService equipmentService = new EquipmentService(equipmentRepo, roomService);
			IEquipmentRelocationService equipmentRelocationService = new EquipmentRelocationService(relocationRepo, roomService, _tasks);
			IRenovationService renovationService = new RenovationService(renovationRepo, _tasks, roomService);
			IRoomAvailabilityService roomAvailabilityService = new RoomAvailabilityService(roomService, renovationService, null);
			IIngredientService ingredientService = new IngredientService(ingredientRepo);
			IMedicationService medicationService = new MedicationService(medicationRepo);
			IMedicationRequestService medicationRequestService = new MedicationRequestService(medicationRequestRepo);

			RoomView roomView = new RoomView(roomService);
			EquipmentView equipmentView = new EquipmentView(equipmentService);
			EquipmentRelocationView equipmentRelocationView = new EquipmentRelocationView(equipmentRelocationService, roomService);
			RenovationView renovationView = new RenovationView(renovationService, roomService, roomAvailabilityService, roomView);
			IngredientView ingredientView = new IngredientView(ingredientService, medicationService, medicationRequestService);
			MedicationView medicationView = new MedicationView(medicationService, ingredientService, medicationRequestService);

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
		}
	}
}
