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
        private static string dataDirectory = Path.Combine("..", "..", "..", "data");
        private const string inputCancelString = "q";
        private static UserAccount user;
        private static bool isRunning = true;

        private class Command
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
            new Command("quit", () => isRunning = false, Enum.GetValues(typeof(UserAccount.AccountType)).Cast<UserAccount.AccountType>()),
            new Command("help", () => ListCommands(user), Enum.GetValues(typeof(UserAccount.AccountType)).Cast<UserAccount.AccountType>()),

            new Command("room-create", () => RoomModel.CreateRoom(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),
            new Command("room-view", () => RoomModel.ViewRoom(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),
            new Command("room-update", () => RoomModel.UpdateRoom(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),
            new Command("room-delete", () => RoomModel.DeleteRoom(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),

            new Command("equipment-search", () => EquipmentModel.Search(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),
            new Command("equipment-filter", () => EquipmentModel.Filter(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),
            new Command("equipment-relocate", () => EquipmentRelocationModel.Relocate(inputCancelString), new[] {UserAccount.AccountType.MANAGER}),

            new Command("appointment-create", () => AppointmentModel.CreateAppointment(inputCancelString, user), new[] {UserAccount.AccountType.DOCTOR, UserAccount.AccountType.PATIENT}),
            new Command("appointment-read", () => AppointmentModel.ReadAppointments(user), new[] {UserAccount.AccountType.DOCTOR, UserAccount.AccountType.PATIENT}),
            new Command("appointment-update", () => AppointmentModel.UpdateAppointment(inputCancelString, user), new[] {UserAccount.AccountType.DOCTOR, UserAccount.AccountType.PATIENT}),
            new Command("appointment-delete", () => AppointmentModel.DeleteAppointment(inputCancelString, user), new[] {UserAccount.AccountType.DOCTOR, UserAccount.AccountType.PATIENT}),
            new Command("appointment-view-start", () => AppointmentModel.ShowNextAppointments(user, inputCancelString), new[] {UserAccount.AccountType.DOCTOR}),
            new Command("appointment-create-rec", () => AppointmentModel.CreateRecommendedAppointment(inputCancelString, user), new[] {UserAccount.AccountType.PATIENT}),

            new Command("anamnesis-search", () => MedicalRecordModel.Search(user, inputCancelString), new[] {UserAccount.AccountType.PATIENT}),

            new Command("patient-account-create", () => UserAccountModel.CreatePatientAccount(inputCancelString), new []{UserAccount.AccountType.SECRETARY}),
            new Command("patient-account-view", () => UserAccountModel.ViewPatientAccounts(), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("patient-account-update", () => UserAccountModel.UpdatePatientAccount(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("patient-account-delete", () => UserAccountModel.DeleteAccount(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("block-patient-account", () => UserAccountModel.BlockPatientAccount(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("unblock-patient-account", () => UserAccountModel.UnblockPatientAccount(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),

            new Command("view-patient-requests", () => RequestModel.ViewRequests(), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("handle-patient-requests", () => RequestModel.HandleRequests(inputCancelString), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("handle-referrals", () => ReferralModel.HandleReferrals(inputCancelString, user), new[] {UserAccount.AccountType.SECRETARY}),
            new Command("create-urgent-appointment", () => AppointmentModel.CreateUrgentAppointment(inputCancelString, user), new[] {UserAccount.AccountType.SECRETARY}),

            new Command("renovation-schedule", () => RenovationModel.NewRenovation(inputCancelString), new [] {UserAccount.AccountType.MANAGER}),

			new Command("medication-create", () => MedicationModel.CreateNewMedicine(inputCancelString), new [] {UserAccount.AccountType.MANAGER}),
			new Command("ingredient-create", () => IngredientModel.Create(inputCancelString), new [] {UserAccount.AccountType.MANAGER}),
			new Command("ingredient-read", () => IngredientModel.Read(inputCancelString), new [] {UserAccount.AccountType.MANAGER}),
			new Command("ingredient-update", () => IngredientModel.Update(inputCancelString), new [] {UserAccount.AccountType.MANAGER}),
			new Command("ingredient-delete", () => IngredientModel.Delete(inputCancelString), new [] {UserAccount.AccountType.MANAGER}),
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

		static void HandleCmdArgs(string[] args) {
			if (args.Count() >= 1) {
				int k = Convert.ToInt32(args[0]);
				string[] paths = new string[k + 1];
				for (int i = 0; i < k; i++)
					paths[i] = "..";
				paths[k] = "data";
				dataDirectory = Path.Combine(paths);
			}
		}

		private static string GetUserCommand() {
			var userCommands = GetCommands(user);
			return EasyInput<string>.Get(
				new List<Func<string, bool>>
				{
					s => userCommands.Count(command => command.CommandName == s) == 1 || s?.Length == 0
				},
				new[]
				{
					"Command not found!\nType help for a list of commands\n"
				},
				inputCancelString
			);
		}

        static void Main(string[] args)
        {
            // === Login ===

            HandleCmdArgs(args);
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

            
            Console.WriteLine("Type help for a list of commands\n\n");
      
            try
            {
                while (isRunning)
                {
                    Console.Write($"{user.Username}>");
                    try
                    {
                        string cmdInput = GetUserCommand();
                        if (cmdInput.Length == 0)
                            continue;

                        Commands.First(cmd => cmd.CommandName == cmdInput).CommandMethod();
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
    }
}
