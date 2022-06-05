using System;
using System.IO;
using HIS.Core.Foundation;
using HIS.Core.RoomModel;
using HIS.Core.EquipmentModel;
using HIS.CLI.View;
using Newtonsoft.Json;
using HIS.Core.EquipmentModel.EquipmentRelocationModel;

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

			IRoomService roomService = new RoomService(roomRepo);
			IEquipmentService equipmentService = new EquipmentService(equipmentRepo);
			EquipmentServiceFacade roomEquipmentServiceFacade = new EquipmentServiceFacade(equipmentService, roomService);
			IEquipmentRelocationService equipmentRelocationService = new EquipmentRelocationService(relocationRepo, _tasks);

			RoomView roomView = new RoomView(roomService);
			EquipmentView equipmentView = new EquipmentView(roomEquipmentServiceFacade);
			EquipmentRelocationView equipmentRelocationView = new EquipmentRelocationView(equipmentRelocationService, roomService);

			try
			{
				equipmentRelocationView.CmdPerform();
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
		}
	}
}
