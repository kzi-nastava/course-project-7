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
        private static string cancelInput = "-q";
        static void Main(string[] args)
        {
            Hospital hospital = new Hospital();
            hospital.Load(dataDirectory);

            // EasyInput.Get

            try
            {
                Console.WriteLine("Input a number in range (0, 4):");
                int res = EasyInput<int>.Get(
                    new List<Func<int, bool>>
                    {
                    num => num > 0,
                    num => num < 4,
                    },
                    new[]
                    {
                    "Number must be greater than 0",
                    "Number must be less than 4",
                    },
                    cancelInput
                );

                Console.WriteLine("Result: " + res);
            }
            catch (InputCancelledException ex)
			{
                Console.WriteLine(ex.Message);
			}

            // EasySelect.Get

            try
			{
                var choices = Enum.GetValues(typeof(Room.RoomType)).Cast<Room.RoomType>().ToList();

                int selection = EasyInput<Room.RoomType>.Select(choices, cancelInput);
                Console.WriteLine("Result: " + choices[selection].ToString());
            }
            catch (InputCancelledException ex)
			{
                Console.WriteLine(ex.Message);
            }
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
    }
}
