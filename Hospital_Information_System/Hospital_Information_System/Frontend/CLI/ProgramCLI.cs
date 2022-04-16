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
                { "-room-update", UpdateRoom },
            };

            //commandMapping["-room-create"]();
            //hospital.Save(dataDirectory);

            commandMapping["-room-update"]();
            hospital.Save(dataDirectory);
        }
        private static void InitHospital()
		{
            hospital.Rooms.Clear();
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
        private static void CreateRoom()
		{
            Room room = new Room();
            try
            {
                InputRoom(room, Enum.GetValues(typeof(RoomProperty)).Cast<RoomProperty>().ToList());
                hospital.Rooms.Add(room);
            }
            catch (InputCancelledException)
			{
			}
        }
        private static void UpdateRoom()
		{
            try
            {
                Console.WriteLine("Select which room to update:");

                Room room = EasyInput<Room>.Select(
                    hospital.Rooms.Where(r => !r.Deleted && r.Type != Room.RoomType.WAREHOUSE).ToList(),
                    r => r.Name,
                    inputCancelString
                );

                Console.WriteLine(room.ToString());
                Console.WriteLine("Select which properties to update:");

                var propertiesToUpdate = EasyInput<RoomProperty>.SelectMultiple(
                    Enum.GetValues(typeof(RoomProperty)).Cast<RoomProperty>().ToList(),
                    e => Enum.GetName(typeof(RoomProperty), e),
                    inputCancelString
                ).ToList();

                InputRoom(room, propertiesToUpdate);
            }
            catch (InputCancelledException)
            {
			}
		}
        internal enum RoomProperty
		{
            NAME,
            FLOOR,
            TYPE,
        }
        private static void InputRoom(Room room, List<RoomProperty> whichProperties)
        {
            Room temp = new Room();

            if (whichProperties.Contains(RoomProperty.NAME))
            {
                Console.WriteLine("Name:");
                temp.Name = EasyInput<string>.Get(
                    new List<Func<string, bool>>
                    {
                        s => s.Count() != 0,
                        s => new Room[]{null, room}.Contains(hospital.Rooms.Find(room => room.Name == s)),
                    },
                    new[]
                    {
                        "Name must not be empty!",
                        "This name is already taken!",
                    },
                    inputCancelString
                );
            }

            if (whichProperties.Contains(RoomProperty.FLOOR))
            {
                Console.WriteLine("Floor:");
                temp.Floor = EasyInput<int>.Get(
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
            }

            if (whichProperties.Contains(RoomProperty.TYPE))
            {
                Console.WriteLine("Room type:");
                temp.Type = EasyInput<Room.RoomType>.Select(
                    Enum.GetValues(typeof(Room.RoomType))
                        .Cast<Room.RoomType>()
                        .Where(e => e != Room.RoomType.WAREHOUSE)
                        .ToList(),
                    inputCancelString
                );
            }

            CopyRoom(room, temp, whichProperties);          
        }
        private static void CopyRoom(Room target, Room source, List<RoomProperty> whichProperties)
		{
            if (whichProperties.Contains(RoomProperty.NAME)) target.Name = source.Name;
            if (whichProperties.Contains(RoomProperty.FLOOR)) target.Floor = source.Floor;
            if (whichProperties.Contains(RoomProperty.TYPE)) target.Type = source.Type;
        }
    }
}
