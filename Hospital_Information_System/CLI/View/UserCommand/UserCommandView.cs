using HIS.Core.PersonModel.UserAccountModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.CLI.View.UserCommand
{
	internal class CommandNotFoundException : Exception
	{
	}

	internal class QuitApplicationException : Exception
	{
	}

	internal abstract class UserCommandView : View
	{
		private Dictionary<string, Action> commandMapping;

		protected UserCommandView(UserAccount user) : base(user)
		{
			commandMapping = new Dictionary<string, Action>
			{
				{ "quit",   () => throw new QuitApplicationException() },
				{ "logout", () => LogOut() },
				{ "help",   () => ShowHelp() }
			};
		}

		protected void AddCommands(Dictionary<string, Action> newCommands)
		{
			newCommands.ToList().ForEach(x => commandMapping.Add(x.Key, x.Value));
		}

		protected void ExecuteCommand(string command)
		{
			try
			{
				commandMapping[command]();
			}
			catch (KeyNotFoundException)
			{
				throw new CommandNotFoundException();
			}
		}

		public void PollCommand()
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(_user.Username + ">");
			string cmd = Console.ReadLine().Trim();
			Console.ResetColor();

			try
			{
				ExecuteCommand(cmd);
			}
			catch (CommandNotFoundException)
			{
				Error("Command not found.");
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private void LogOut()
		{
			var newAccount = new UserAccount(UserAccount.AccountType.LOGGED_OUT);
			_user.Username = newAccount.Username;
			_user.Password = newAccount.Password;
			_user.Blocked = newAccount.Blocked;
			_user.Deleted = newAccount.Deleted;
			_user.Id = newAccount.Id;
			_user.Person = newAccount.Person;
			_user.Type = newAccount.Type;
		}

		private void ShowHelp()
		{
			foreach (var kv in commandMapping)
			{
				Print(kv.Key);
			}
		}
	}
}
