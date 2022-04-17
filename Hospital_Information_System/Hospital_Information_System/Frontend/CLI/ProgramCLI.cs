using System;
using System.IO;
using System.Collections.Generic;
using HospitalIS.Backend;

namespace HospitalIS.Frontend.CLI
{ 
    internal partial class CLIProgram
    {
        private static readonly string dataDirectory = Path.Combine("..", "..", "..", "data");
        private static readonly string inputCancelString = "-q";
        private static Hospital hospital;
        static void Main()
        {
			hospital = new Backend.Hospital();
            //hospital.Load(dataDirectory);
            InitHospital();
            hospital.Save(dataDirectory);

            var commandMapping = new Dictionary<string, Action>
            {
                { "-room-create", () => RoomModel.CreateRoom(hospital, inputCancelString) },
                { "-room-update", () => RoomModel.UpdateRoom(hospital, inputCancelString) },
                { "-room-delete", () => RoomModel.DeleteRoom(hospital, inputCancelString) },
            };

            commandMapping["-room-create"]();
            commandMapping["-room-update"]();
            commandMapping["-room-delete"]();

            hospital.Save(dataDirectory);
        }
        private static void InitHospital()
        {
            hospital.Add(new Room(0, Room.RoomType.WAREHOUSE, "Warehouse"));

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
                        hospital.Add(new Room(floor, rc.Key, i));
                    }
                }
            }
        }
    }
}
