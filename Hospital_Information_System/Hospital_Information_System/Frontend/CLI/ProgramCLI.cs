using HospitalIS.Backend.Room;
using HospitalIS.Frontend.CLI.View;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace HospitalIS.Frontend.CLI
{
	internal static class CLIProgram
    {
        private static readonly string dataDir = Path.Combine("..", "..", "..", "data") + Path.DirectorySeparatorChar;

        static void Main(string[] args)
        {
            IRoomRepository roomRepo = default;
            IEquipmentRepository equipmentRepo = default;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new EquipmentDictionaryIntConverter(equipmentRepo) },
                PreserveReferencesHandling = PreserveReferencesHandling.None,
            };

            roomRepo = new RoomJSONRepository(dataDir + "db_rooms.json", JsonConvert.DefaultSettings());
            equipmentRepo = new EquipmentJSONRepository(dataDir + "db_equipment.json", JsonConvert.DefaultSettings());
            RoomService roomService = new RoomService(roomRepo);
            RoomView roomView = new RoomView(roomService);

            roomView.CmdRoomCreate();

            roomRepo.Save();
        }
    }
}
