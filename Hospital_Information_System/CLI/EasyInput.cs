using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.CLI.View
{
	/// <summary>
	/// Thrown when the user inputs an exit string. Signifies that the operation has been cancelled.
	/// </summary>
	public class InputCancelledException : Exception
	{
		public InputCancelledException() : base("Input cancelled.")
		{

		}

		public InputCancelledException(string message) : base(message)
		{
		}

		public InputCancelledException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	/// <summary>
	/// Thrown when Select or any of its variants is called on an empty sequence.
	/// </summary>
	public class NothingToSelectException : Exception
	{
		public NothingToSelectException() : base("Nothing to select!")
		{

		}

		public NothingToSelectException(string errorMessage) : base(errorMessage)
		{
		}

		public NothingToSelectException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	/// <summary>
	/// EasyInput is a helper class for querying user input.
	/// </summary>
	internal abstract class EasyInput<T>
	{
		private static void WriteLineError(string err)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(err);
			Console.ResetColor();
		}

		/// <summary>
		/// Continuously asks for user input until it satisfies all rules or the user cancels the operation.
		/// </summary>
		public static T Get(IList<Func<T, bool>> rules, IList<string> errorMsg, string cancel)
		{
			return Get(rules, errorMsg, cancel, s => (T)Convert.ChangeType(s, typeof(T)));
		}

		public static T Get(string cancel)
		{
			return Get(new List<Func<T, bool>>(), new string[] { }, cancel);
		}

		public static T Get(IList<Func<T, bool>> rules, IList<string> errorMsg, string cancel, Func<string, T> conversionFunction)
		{
			T result = default;
			while (true)
			{
				string input = Console.ReadLine();
				if (input == cancel)
				{
					throw new InputCancelledException();
				}

				try
				{
					result = conversionFunction(input);
				}
				catch
				{
					WriteLineError("Invalid input.");
					continue;
				}

				int brokenRuleIndex = GetBrokenRuleIndex(rules, result);

				if (brokenRuleIndex != -1)
				{
					WriteLineError(errorMsg[brokenRuleIndex]);
				}
				else
				{
					break;
				}
			}

			return result;
		}

		private static int GetBrokenRuleIndex(IEnumerable<Func<T, bool>> rules, T input)
		{
			for (int i = 0; i < rules.Count(); i++)
			{
				if (!rules.ElementAt(i).Invoke(input))
					return i;
			}
			return -1;
		}

		public static T Select(IEnumerable<T> elements, IEnumerable<Func<T, bool>> rules, IEnumerable<string> errorMsg, Func<T, string> toStrFunc, string cancel)
		{
			if (!elements.Any())
			{
				throw new NothingToSelectException();
			}

			for (int i = 0; i < elements.Count(); i++)
			{
				Console.WriteLine(i + ". " + toStrFunc.Invoke(elements.ElementAt(i)));
			}

			int selection = -1;

			try
			{
				while (true)
				{
					selection = EasyInput<int>.Get(
						new List<Func<int, bool>>
						{
							n => n >= 0,
							n => n < elements.Count(),
						},
						new[]
						{
							"Selection must be greater than 0.",
							"Selection must be less than " + elements.Count() + ".",
						},
						cancel
					);

					int brokenRuleIndex = GetBrokenRuleIndex(rules, elements.ElementAt(selection));
					if (brokenRuleIndex != -1)
					{
						WriteLineError(errorMsg.ElementAt(brokenRuleIndex));
					}
					else
					{
						break;
					}
				}
			}
			catch
			{
				throw;
			}

			return elements.ElementAt(selection);
		}

		public static T Select(IEnumerable<T> elements, Func<T, string> toStrFunc, string cancel)
		{
			return Select(elements, new List<Func<T, bool>>(), new string[] { }, toStrFunc, cancel);
		}

		public static T Select(IEnumerable<T> elements, string cancel)
		{
			return Select(elements, elem => elem.ToString(), cancel);
		}

		public static bool YesNo(string cancel)
		{
			Console.WriteLine("[Y/N]");
			string s = EasyInput<string>.Get(
				new List<Func<string, bool>> { s => string.Equals(s, "y", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "n", StringComparison.OrdinalIgnoreCase) },
				new[] { "Must be [Y]es or [N]o." },
				cancel
			);

			s = s.ToLower();
			return s == "y";
		}

		public static IList<T> SelectMultiple(IList<T> elements, string cancel)
		{
			return SelectMultiple(elements, e => e.ToString(), cancel);
		}

		/// <summary>
		/// Select multiple elements from the given list, separated by whitespace. 
		/// Inputing the same item twice cancels its input.
		/// Empty input implies end of selection.
		/// </summary>
		public static IList<T> SelectMultiple(IList<T> elements, Func<T, string> toStrFunc, string cancel)
		{
			if (elements.Count == 0)
			{
				throw new NothingToSelectException();
			}

			// TODO @magley: Find a way to not print all the elements at once, for the sake of brievity (idea: ranges, pages, ...)

			bool[] isSelected = new bool[elements.Count];

			while (true)
			{
				PrintWithSelection(elements, toStrFunc, isSelected);
				string input = Console.ReadLine();

				if (input == cancel)
				{
					throw new InputCancelledException();
				}
				else if (input == "")
				{
					break;
				}

				input
					.Split(' ')
					.Where(x => int.TryParse(x, out _))
					.Select(int.Parse)
					.Where(x => x >= 0 && x < elements.Count)
					.ToList()
					.ForEach(i => { isSelected[i] ^= true; });
			}

			return elements.Where(elem => isSelected[elements.IndexOf(elem)]).ToList();
		}

		private static void PrintWithSelection(IList<T> elements, Func<T, string> toStrFunc, IList<bool> isSelected)
		{
			// [x] 1. Room 1
			// [ ] 2. Room 2
			// etc.
			for (int i = 0; i < elements.Count; i++)
			{
				Console.WriteLine($"[{(isSelected[i] ? 'x' : ' ')}] {i}. {toStrFunc.Invoke(elements[i])}");
			}
		}
	}
}
