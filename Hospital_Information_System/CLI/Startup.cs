using HIS.Core.EquipmentModel;
using HIS.Core.RoomModel;
using HIS.CLI.View;
using Newtonsoft.Json;
using System.IO;

namespace HIS.CLI
{
	static class Startup
	{
		private static readonly string dataDir = Path.Combine("..", "..", "..", "data") + Path.DirectorySeparatorChar;
		private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None };

		static void Main()
		{
			IEquipmentRepository equipmentRepo = new EquipmentJSONRepository(dataDir + "db_equipment.json", jsonSettings);
			IRoomRepository roomRepo = new RoomJSONRepository(dataDir + "db_rooms.json", jsonSettings);

			IRoomService roomService = new RoomService(roomRepo);
			IEquipmentService equipmentService = new EquipmentService(equipmentRepo);
			EquipmentServiceFacade roomEquipmentServiceFacade = new EquipmentServiceFacade(equipmentService, roomService);

			RoomView roomView = new RoomView(roomService);
			EquipmentView equipmentView = new EquipmentView(roomEquipmentServiceFacade);

			equipmentView.CmdFilter();

			roomRepo.Save();
		}
	}
}
