using System;
using System.IO;
using Hospital_Information_System.Backend;

namespace Hospital_Information_System.Frontend.CLI
{
	internal class ProgramCLI
	{
        private static string dataDirectory = Path.Combine("..", "..", "..", "data");
        static void Main(string[] args)
        {
            Hospital hospital = new Hospital();

            // Write from memory into disk.

            //InitHospital(ref hospital);
            //hospital.Save(dataDirectory);

            // Read from disk into memory.

            hospital.Load(dataDirectory);
            Console.WriteLine(hospital);
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
