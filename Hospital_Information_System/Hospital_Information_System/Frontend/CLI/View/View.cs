using System;

namespace HospitalIS.Frontend.CLI.View
{
	internal abstract class View
	{
		protected static string _cancel = "-q";

		protected void Hint(string value)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
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
			Console.Write(value);
		}
	}
}
