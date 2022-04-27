using System;
using System.IO;
using System.Collections.Generic;
using HospitalIS.Backend;
using HospitalIS.Frontend.CLI.Model;

namespace HospitalIS.Frontend.CLI
{
    internal static class CLIProgram
    {
        private static readonly string dataDirectory = Path.Combine("..", "..", "..", "data");
        private const string inputCancelString = "-q";
        private static UserAccount user;

        private static readonly Dictionary<string, Action> commandMapping = new Dictionary<string, Action>
        {
            { "-room-create", () => RoomModel.CreateRoom(inputCancelString) },
            { "-room-update", () => RoomModel.UpdateRoom(inputCancelString) },
            { "-room-delete", () => RoomModel.DeleteRoom(inputCancelString) },
            { "-equipment-search", () => EquipmentModel.Search(inputCancelString) },
            { "-equipment-filter", () => EquipmentModel.Filter(inputCancelString) },
            { "-equipment-relocate", () => EquipmentRelocationModel.Relocate(inputCancelString) },
            { "-appointment-create", () => AppointmentModel.CreateAppointment(inputCancelString, user) },
            { "-appointment-update", () => AppointmentModel.UpdateAppointment(inputCancelString, user) },
            { "-appointment-delete", () => AppointmentModel.DeleteAppointment(inputCancelString, user) },
        };

        static void Main()
        {
            //InitHospital();
            //IS.Instance.Save(dataDirectory);

            try
            {
                IS.Instance.Load(dataDirectory);
                user = UserAccountModel.AttemptLogin("bowen", "123");
                commandMapping["-appointment-update"]();
                //commandMapping["-appointment-create"]();
            }
            catch (UserAccountForcefullyBlockedException e)
            {
                Console.WriteLine(e.Message);
                user = null;
            }
            finally
            {
                IS.Instance.Save(dataDirectory);
            }
        }

		private static void InitHospital()
		{
            IS.Instance.RoomRepo.Add(new Room(0, Room.RoomType.WAREHOUSE, "Warehouse"));

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
                        IS.Instance.RoomRepo.Add(new Room(floor, rc.Key, i));
                    }
                }
            }

            for (int i = 0; i < Enum.GetValues(typeof(Equipment.EquipmentType)).Length; i++)
            {
                for (int j = 0; j < Enum.GetValues(typeof(Equipment.EquipmentUse)).Length; j++)
				{
                    Equipment eq = new Equipment((Equipment.EquipmentType)i, (Equipment.EquipmentUse)j);
                    IS.Instance.EquipmentRepo.Add(eq);
				}
            }

            for (int i = 0; i < IS.Instance.Hospital.Rooms.Count * 2; i++)
			{
                IS.Instance.RoomRepo.Add(
                    IS.Instance.Hospital.Rooms[i % IS.Instance.Hospital.Rooms.Count],
                    IS.Instance.Hospital.Equipment[i % IS.Instance.Hospital.Equipment.Count],
                    new Random().Next(1, 10)
                );
            }
        }
    }
}
