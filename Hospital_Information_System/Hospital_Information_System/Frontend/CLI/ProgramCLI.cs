using System;
using System.IO;
using System.Collections.Generic;
using Hospital_Information_System.Backend;
using System.Linq;

namespace Hospital_Information_System.Frontend.CLI
{
	internal class ProgramCLI
	{
        private static string dataDirectory = Path.Combine("..", "..", "..", "data");
        private static string inputCancelString = "-q";
        private static Hospital hospital;
        static void Main(string[] args)
        {
            hospital = new Hospital();
            hospital.Load(dataDirectory);

			var commandMapping = new Dictionary<string, Action>
			{
				{ "-room-create", CreateRoom }
			};

            commandMapping["-room-create"]();
		}
        private static void InitHospital(ref Hospital hospital)
		{
            hospital.Rooms.Add(new Room(0, Room.RoomType.WAREHOUSE, "Warehouse"));

            const int floorNo = 4;
            const int bathroomsPerFloor = 7;
            const int examinationRoomsPerFloor = 5;
            const int operationRoomsPerFloor = 3;
            const int recoveryRoomsPerFloor = 4;

            for (int floor = 0; floor < floorNo; floor++)
            {
                for (int i = 0; i < bathroomsPerFloor; i++)
                {
                    hospital.Rooms.Add(new Room(floor, Room.RoomType.BATHROOM, i));
                }
                for (int i = 0; i < examinationRoomsPerFloor; i++)
                {
                    hospital.Rooms.Add(new Room(floor, Room.RoomType.EXAMINATION, i));
                }
                for (int i = 0; i < operationRoomsPerFloor; i++)
                {
                    hospital.Rooms.Add(new Room(floor, Room.RoomType.OPERATION, i));
                }
                for (int i = 0; i < recoveryRoomsPerFloor; i++)
                {
                    hospital.Rooms.Add(new Room(floor, Room.RoomType.RECOVERY, i));
                }
            }
        }
        private static void CreateRoom()
		{
            var room = InputRoom();
            if (room != null)
            {
                hospital.Rooms.Add(room);
                hospital.Save(dataDirectory);
            }
        }
        private static Room InputRoom()
        {
            try
            {
                Console.WriteLine("Name:");
                var name = EasyInput<string>.Get(
                    new List<Func<string, bool>>
                    {
                        s => s.Count() != 0,
                        s => hospital.Rooms.Find(room => room.Name == s) == null,
                    },
                    new[]
                    {
                        "Name must not be empty!",
                        "This name is already taken!",
                    },
                    inputCancelString
                );

                Console.WriteLine("Floor:");
                var floor = EasyInput<int>.Get(
                    new List<Func<int, bool>>
                    {
                        n => n >= 0
                    },
                    new[]
                    {
                        "Floor must be a non-negative integer!",
                    },
                    inputCancelString
                );

                Console.WriteLine("Room type:");
                var type = EasyInput<Room.RoomType>.Select(
					Enum.GetValues(typeof(Room.RoomType)).Cast<Room.RoomType>().ToList().Where(e => e != Room.RoomType.WAREHOUSE).ToList(),
                    inputCancelString
                );

                return new Room(floor, type, name);
            }
            catch (InputCancelledException)
			{
                return null;
			}
        }
    }
}
