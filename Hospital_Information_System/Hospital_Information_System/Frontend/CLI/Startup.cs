using HospitalIS.Backend.RoomModel;
using HospitalIS.Backend.EquipmentModel;
using HospitalIS.Frontend.CLI.View;
using Newtonsoft.Json;
using System.IO;

namespace HospitalIS.Frontend.CLI
{
	internal static class Startup
	{
		private static readonly string dataDir = Path.Combine("..", "..", "..", "data") + Path.DirectorySeparatorChar;
		private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None };

		static void Main()
		{
			IEquipmentRepository equipmentRepo = new EquipmentJSONRepository(dataDir + "db_equipment.json", jsonSettings);
			IRoomRepository roomRepo = new RoomJSONRepository(dataDir + "db_rooms.json", jsonSettings);

			IRoomService roomService = new RoomService(roomRepo);
			IEquipmentService equipmentService = new EquipmentService(equipmentRepo);
			RoomEquipmentServiceFacade roomEquipmentServiceFacade = new RoomEquipmentServiceFacade(equipmentService, roomService);

			RoomView roomView = new RoomView(roomService);
			EquipmentView equipmentView = new EquipmentView(roomEquipmentServiceFacade);

			roomRepo.Save();
		}
	}
}
