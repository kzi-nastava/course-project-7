using HospitalIS.Backend.Room;
using HospitalIS.Frontend.CLI.View;
using Newtonsoft.Json;
using System.IO;

namespace HospitalIS.Frontend.CLI
{
	internal static class CLIProgram
	{
		private static readonly string dataDir = Path.Combine("..", "..", "..", "data") + Path.DirectorySeparatorChar;
		private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None };

		static void Main()
		{
			IEquipmentRepository equipmentRepo = new EquipmentJSONRepository(dataDir + "db_equipment.json", jsonSettings);
			IRoomRepository roomRepo = new RoomJSONRepository(dataDir + "db_rooms.json", jsonSettings);

			RoomService roomService = new RoomService(roomRepo);
			RoomView roomView = new RoomView(roomService);

			roomView.CmdRoomView();

			roomRepo.Save();
		}
	}
}
