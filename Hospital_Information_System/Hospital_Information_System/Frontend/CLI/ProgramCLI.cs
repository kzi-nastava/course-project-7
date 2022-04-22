using System;
using System.IO;
using System.Collections.Generic;
using HospitalIS.Backend;
using HospitalIS.Backend.Repository;
using HospitalIS.Frontend.CLI.Model;

namespace HospitalIS.Frontend.CLI
{
    internal static class CLIProgram
    {
        private static readonly string dataDirectory = Path.Combine("..", "..", "..", "data");
        private const string inputCancelString = "-q";

        private static readonly Dictionary<string, Action> commandMapping = new Dictionary<string, Action>
        {
            { "-room-create", () => RoomModel.CreateRoom(inputCancelString) },
            { "-room-update", () => RoomModel.UpdateRoom(inputCancelString) },
            { "-room-delete", () => RoomModel.DeleteRoom(inputCancelString) },
            { "-equipment-search", () => EquipmentModel.Search(inputCancelString) },
            { "-equipment-filter", () => EquipmentModel.Filter(inputCancelString) },
            { "-equipment-relocate", () => EquipmentRelocationModel.Relocate(inputCancelString) },
        };

        static void Main()
        {
            //InitHospital();
            //Hospital.Instance.Save(dataDirectory);

            Hospital.Instance.Load(dataDirectory);
            commandMapping["-equipment-relocate"]();
            Hospital.Instance.Save(dataDirectory);
        }

		private static void InitHospital()
		{
            Hospital.Instance.Add(new Room(0, Room.RoomType.WAREHOUSE, "Warehouse"));

            const int floorNo = 3;
            var roomCountPerFloor = new List<KeyValuePair<Room.RoomType, int>>
            {
                new KeyValuePair<Room.RoomType, int>(Room.RoomType.BATHROOM, 3),
                new KeyValuePair<Room.RoomType, int>(Room.RoomType.EXAMINATION, 2),
                new KeyValuePair<Room.RoomType, int>(Room.RoomType.OPERATION, 1),
                new KeyValuePair<Room.RoomType, int>(Room.RoomType.RECOVERY, 1),
            };

            for (int floor = 0; floor < floorNo; floor++)
            {
                foreach (var rc in roomCountPerFloor)
                {
                    for (int i = 0; i < rc.Value; i++)
                    {
                        Hospital.Instance.Add(new Room(floor, rc.Key, i));
                    }
                }
            }

            for (int i = 0; i < Enum.GetValues(typeof(Equipment.EquipmentType)).Length; i++)
            {
                for (int j = 0; j < Enum.GetValues(typeof(Equipment.EquipmentUse)).Length; j++)
				{
                    Equipment eq = new Equipment((Equipment.EquipmentType)i, (Equipment.EquipmentUse)j);
                    Hospital.Instance.Add(eq);
				}
            }

            for (int i = 0; i < Hospital.Instance.Rooms.Count * 2; i++)
			{
                RoomRepository.AddEquipment(
                    Hospital.Instance.Rooms[i % Hospital.Instance.Rooms.Count],
                    Hospital.Instance.Equipment[i % Hospital.Instance.Equipment.Count],
                    new Random().Next(1, 10)
                );
            }
        }
    }
}
