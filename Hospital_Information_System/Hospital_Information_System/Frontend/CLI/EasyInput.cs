using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Frontend.CLI
{
	/// <summary>
	/// This exception does not imply that an error happened but rather that the user cancelled his input.
	/// </summary>
	public class InputCancelledException : Exception
	{
		public InputCancelledException() : base("Input cancelled.")
		{

		}
	}

	public class NothingToSelectException : Exception
	{
		public NothingToSelectException() : base("Could not select from empty list.")
		{

		}
	}

	/// <summary>
	/// EasyInput is a helper class for querying user input.
	/// </summary>
	internal abstract class EasyInput<T>
	{
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
					Console.WriteLine("Invalid input.");
					continue;
				}

				int brokenRuleIndex = GetBrokenRuleIndex(rules, result);

				if (brokenRuleIndex != -1)
				{
					Console.WriteLine(errorMsg[brokenRuleIndex]);
					continue;
				}
				else
				{
					break;
				}
			}

			return result;
		}

		private static int GetBrokenRuleIndex(IList<Func<T, bool>> rules, T input)
		{
			for (int i = 0; i < rules.Count; i++)
			{
				if (rules[i].Invoke(input) == false)
					return i;
			}
			return -1;
		}

		/// <summary>
		/// Select one element from the given list.
		/// </summary>
		public static T Select(IList<T> elements, IList<Func<T, bool>> rules, IList<string> errorMsg, Func<T, string> toStrFunc, string cancel)
		{
			if (elements.Count == 0)
            {
				throw new NothingToSelectException();
            }

			for (int i = 0; i < elements.Count; i++)
			{
				Console.WriteLine(i + ". " + toStrFunc.Invoke(elements[i]));
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
						n => n < elements.Count,
						},
						new[]
						{
						"Selection must be greater than 0.",
						"Selection must be less than " + elements.Count + ".",
						},
						cancel
					);

					int brokenRuleIndex = GetBrokenRuleIndex(rules, elements[selection]);
					if (brokenRuleIndex != -1)
					{
						Console.WriteLine(errorMsg[brokenRuleIndex]);
						continue;
					}
					else
					{
						break;
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return elements[selection];
		}

		public static T Select(IList<T> elements, Func<T, string> toStrFunc, string cancel)
		{
			return Select(elements, new List<Func<T, bool>>(), new string[] {}, toStrFunc, cancel);
		}

		public static T Select(IList<T> elements, string cancel)
		{
			return Select(elements, (elem => elem.ToString()), cancel);
		}

		public static IList<T> SelectMultiple(IList<T> elements, string cancel)
		{
			return SelectMultiple(elements, e => e.ToString(), cancel);
		}
		/// <summary>
		/// Select multiple elements from the given list, separated by whitespace. An empty input implies end of selection.
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
				printWithSelection(elements, toStrFunc, isSelected);
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

			return (from e 
					in elements 
					where isSelected[elements.IndexOf(e)] 
					select e
			).ToList(); 
		}
		private static void printWithSelection(IList<T> elements, Func<T, string> toStrFunc, IList<bool> isSelected)
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
