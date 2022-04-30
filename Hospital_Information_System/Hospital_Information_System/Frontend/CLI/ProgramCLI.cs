using System;
using System.IO;
using System.Collections.Generic;
using HospitalIS.Backend;
using HospitalIS.Frontend.CLI.Model;
using HospitalIS.Backend.Controller;
using System.Linq;

namespace HospitalIS.Frontend.CLI
{
    internal static class CLIProgram
    {
        private static readonly string dataDirectory = Path.Combine("..", "..", "..", "data");
        private const string inputCancelString = "-q";
        private static UserAccount user;
        private static bool isRunning = true;

        private struct Command
        {
            public string CommandName;
            public Action CommandMethod;
            public List<UserAccount.AccountType> AvailableTo;

            public Command(string name, Action methodCall, UserAccount.AccountType[] availableTo)
            {
                CommandName = name;
                CommandMethod = methodCall;
                AvailableTo = availableTo.ToList();
            }

            public Command(string name, Action methodCall, IEnumerable<UserAccount.AccountType> availableTo)
            {
                CommandName = name;
                CommandMethod = methodCall;
                AvailableTo = availableTo.ToList();
            }
        }

        private static List<Command> Commands =
        new List<Command>{
            new Command("-quit", () => isRunning = false, Enum.GetValues(typeof(UserAccount.AccountType)).Cast<UserAccount.AccountType>()),
            new Command("-help", () => ListCommands(user), Enum.GetValues(typeof(UserAccount.AccountType)).Cast<UserAccount.AccountType>()),

            new Command("-room-create", () => RoomModel.CreateRoom(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),
            new Command("-room-view", () => RoomModel.ViewRoom(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),
            new Command("-room-update", () => RoomModel.UpdateRoom(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),
            new Command("-room-delete", () => RoomModel.DeleteRoom(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),

            new Command("-equipment-search", () => EquipmentModel.Search(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),
            new Command("-equipment-filter", () => EquipmentModel.Filter(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),
            new Command("-equipment-relocate", () => EquipmentRelocationModel.Relocate(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),

            new Command("-appointment-create", () => AppointmentModel.CreateAppointment(inputCancelString, user), new[] {UserAccount.AccountType.DOCTOR, UserAccount.AccountType.PATIENT}),
            new Command("-appointment-read", () => AppointmentModel.ReadAppointments(user), new[] {UserAccount.AccountType.DOCTOR, UserAccount.AccountType.PATIENT}),
            new Command("-appointment-update", () => AppointmentModel.UpdateAppointment(inputCancelString, user), new[] {UserAccount.AccountType.DOCTOR, UserAccount.AccountType.PATIENT}),
            new Command("-appointment-delete", () => AppointmentModel.DeleteAppointment(inputCancelString, user), new[] {UserAccount.AccountType.DOCTOR, UserAccount.AccountType.PATIENT}),
            new Command("-appointment-view-start", () => AppointmentModel.ShowNextAppointments(user, inputCancelString), new[] {UserAccount.AccountType.DOCTOR}),
        
            new Command("-patient-create", () => UserAccountModel.CreatePatientAccount(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("-patient-update", () => UserAccountModel.UpdatePatientAccount(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("-patient-delete", () => UserAccountModel.DeleteAccount(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}), // Patient-only?
            new Command("-patient-block", () => UserAccountModel.BlockPatientAccount(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("-delete-request-approve", () => RequestModel.ApproveDeleteRequest(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("-delete-request-deny", () => RequestModel.DenyDeleteRequest(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("-update-request-approve", () => RequestModel.ApproveUpdateRequest(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("-update-request-deny", () => RequestModel.DenyUpdateRequest(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),
        };

        static List<Command> GetCommands(UserAccount user)
		{
            return Commands.Where(cmd => cmd.AvailableTo.Contains(user.Type)).ToList();
		}

        static void ListCommands(UserAccount user)
		{
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            foreach (var cmd in GetCommands(user))
			{
                Console.WriteLine(cmd.CommandName);
			}
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        static void Main()
        {
            // === Login ===

            IS.Instance.Load(dataDirectory);

            try
            {
                user = UserAccountModel.Login(inputCancelString);
            }
            catch (InputCancelledException)
			{
                return;
			}

            // === Use program ===

            var userCommands = GetCommands(user);
            Console.WriteLine("Type -help for a list of commands\n\n");
      
            try
            {
                while (isRunning)
                {
                    Console.Write($"{user.Username}>");
                    try
                    {
                        string cmdInput = EasyInput<string>.Get(
                            new List<Func<string, bool>>
                            {
                            s => userCommands.Count(command => command.CommandName == s) == 1 || s?.Length == 0
							},
                            new[]
                            {
                            "Command not found!\nType -help for a list of commands\n"
                            },
                            inputCancelString
                        );

                        if (cmdInput.Length == 0)
						{
                            continue;
						}

                        userCommands.First(cmd => cmd.CommandName == cmdInput).CommandMethod();
                    }
                    catch (InputCancelledException)
                    {
                        Console.WriteLine("\n^C\n");
                    }
                    catch (UserAccountForcefullyBlockedException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                }
            }
            finally
            {
                IS.Instance.Save(dataDirectory);
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
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
