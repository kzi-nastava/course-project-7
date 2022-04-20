using System;
using System.IO;
using System.Collections.Generic;
using HospitalIS.Backend;
using HospitalIS.Frontend.CLI.Model;
using System.Threading;

namespace HospitalIS.Frontend.CLI
{ 
    internal partial class CLIProgram
    {
        private static readonly string dataDirectory = Path.Combine("..", "..", "..", "data");
        private static readonly string inputCancelString = "-q";
        private static Hospital hospital;

        private static readonly Dictionary<string, Action> commandMapping = new Dictionary<string, Action>
        {
            { "-room-create", () => RoomModel.CreateRoom(hospital, inputCancelString) },
            { "-room-update", () => RoomModel.UpdateRoom(hospital, inputCancelString) },
            { "-room-delete", () => RoomModel.DeleteRoom(hospital, inputCancelString) },
            { "-equipment-search", () => EquipmentModel.Search(hospital, inputCancelString) },
            { "-equipment-filter", () => EquipmentModel.Filter(hospital, inputCancelString) },
            { "-equipment-relocate", () => EquipmentRelocartionModel.Relocate(hospital, inputCancelString) },
        };

        static void SayHelloAtTime(DateTime when)
		{
            Thread.Sleep((int)(when - DateTime.Now).TotalMilliseconds);
            Console.WriteLine("Hello from other thread!");
		}
        static void Main()
        {
            hospital = new Backend.Hospital();

            //InitHospital();
            //hospital.Save(dataDirectory);
            hospital.Load(dataDirectory);

            commandMapping["-equipment-relocate"]();
            hospital.Save(dataDirectory);

            //Console.WriteLine("Now!");
            //Thread t = new Thread(new ThreadStart(() => SayHelloAtTime(DateTime.Now.AddSeconds(3))));
            //t.Start();
        }
        private static void InitHospital()
        {
            hospital.Add(new Room(0, Room.RoomType.WAREHOUSE, "Warehouse"));

            const int floorNo = 4;
            var roomCountPerFloor = new List<KeyValuePair<Room.RoomType, int>>
            {
                new KeyValuePair<Room.RoomType, int>(Room.RoomType.BATHROOM, 4),
                new KeyValuePair<Room.RoomType, int>(Room.RoomType.EXAMINATION, 2),
                new KeyValuePair<Room.RoomType, int>(Room.RoomType.OPERATION, 1),
                new KeyValuePair<Room.RoomType, int>(Room.RoomType.RECOVERY, 2),
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

            // ----------------------------------------------------------------------

            for (int i = 0; i < 100; i++)
			{
                var type = (Equipment.EquipmentType)(i % Enum.GetValues(typeof(Equipment.EquipmentType)).Length);
                var use = (Equipment.EquipmentUse)(i % Enum.GetValues(typeof(Equipment.EquipmentUse)).Length);

                Equipment eq = new Equipment(type, use);

                hospital.Add(eq);
                hospital.Rooms[i % hospital.Rooms.Count].Equipment.Add(eq);
            }
        }
    }
}
