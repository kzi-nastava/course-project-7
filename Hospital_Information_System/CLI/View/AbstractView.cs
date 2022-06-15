using HIS.Core.PersonModel.UserAccountModel;
using System;

namespace HIS.CLI.View
{
	internal abstract class AbstractView
	{
		protected static string _cancel = "-q";
		public static UserAccount User { get; protected set; } = new UserAccount(UserAccount.AccountType.LOGGED_OUT);

		protected void Hint(string value)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(value);
			Console.ResetColor();
		}

		protected void Error(string value)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(value);
			Console.ResetColor();
		}

		protected void Print(string value)
		{
			Console.WriteLine(value);
		}
	}
}
