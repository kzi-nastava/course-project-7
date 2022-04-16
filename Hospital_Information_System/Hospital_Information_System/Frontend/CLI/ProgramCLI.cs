using System;
using System.IO;
using System.Collections.Generic;
using Hospital_Information_System.Backend;
using System.Linq;

namespace Hospital_Information_System.Frontend.CLI
{
	internal class ProgramCLI
	{
        private static readonly string dataDirectory = Path.Combine("..", "..", "..", "data");
        private static readonly string inputCancelString = "-q";
        private static Hospital hospital;
        static void Main()
        {
            hospital = new Hospital();
            hospital.Load(dataDirectory);
            //InitHospital();
            //hospital.Save(dataDirectory);

            var commandMapping = new Dictionary<string, Action>
			{
				{ "-room-create", CreateRoom },
                { "-room-delete", DeleteRoom },
			};

            //commandMapping["-room-create"]();
            commandMapping["-room-delete"]();

            hospital.Save(dataDirectory);
        }
        private static void InitHospital()
		{
            hospital.Rooms.Add(new Room(0, Room.RoomType.WAREHOUSE, "Warehouse"));

            const int floorNo = 4;
			var roomCountPerFloor = new List<KeyValuePair<Room.RoomType, int>>
			{
				new KeyValuePair<Room.RoomType, int>(Room.RoomType.BATHROOM, 7),
                new KeyValuePair<Room.RoomType, int>(Room.RoomType.EXAMINATION, 5),
                new KeyValuePair<Room.RoomType, int>(Room.RoomType.OPERATION, 3),
                new KeyValuePair<Room.RoomType, int>(Room.RoomType.RECOVERY, 4),
            };

			for (int floor = 0; floor < floorNo; floor++)
            {
                foreach (var rc in roomCountPerFloor)
				{
                    for (int i = 0; i < rc.Value; i++)
					{
                        hospital.Rooms.Add(new Room(floor, rc.Key, i));
                    }
				}
            }
        }
        private static void DeleteRoom()
		{
            Console.WriteLine("Enter index of rooms to delete, separated by whitespace.\nEnter a newline to finish.");

            try
            {
                var roomsToDelete = EasyInput<Room>.SelectMultiple(
                    hospital.Rooms.Where(r => !r.Deleted && r.Type != Room.RoomType.WAREHOUSE).ToList(), 
                    r => r.Name, 
                    inputCancelString
                );

                foreach (var room in roomsToDelete)
                {
                    room.Deleted = true;
                }
            }
            catch (InputCancelledException)
			{
			}
		}

        private static void PrintRooms(List<Room> markedRooms)
		{
            int i = 0;
            foreach (var room in hospital.Rooms)
			{
                Console.Write($"[{(markedRooms.Contains(room) ? 'x' : ' ')}]");
                Console.Write($"{i++} {room.Name}\n");
            }
		}

        private static void CreateRoom()
		{
            var room = InputRoom();
            if (room != null)
            {
                hospital.Rooms.Add(room);
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
