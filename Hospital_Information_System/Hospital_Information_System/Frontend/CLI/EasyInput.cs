using System;
using System.Collections.Generic;

namespace Hospital_Information_System.Frontend.CLI
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
	/// <summary>
	/// EasyInput is a helper class for querying user input.
	/// </summary>
	internal abstract class EasyInput<T>
	{
		/// <summary>
		/// Get continuously asks for user input until it satisfies all rules or the user cancels the operation.
		/// </summary>
		/// <param name="rules">List of predicates the result must satisfy.</param>
		/// <param name="errorMsg">Message that gets printed in case the corresponding rule is not satisfied.</param>
		/// <param name="cancel">String upon whose input cancels the operation.</param>
		/// <returns>Valid input converted into <typeparamref name="T"/>.</returns>
		/// <exception cref="InputCancelledException">Thrown if the user inputs the exact value for 'cancel'.</exception>
		public static T Get(IList<Func<T, bool>> rules, IList<string> errorMsg, string cancel)
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
					result = (T)Convert.ChangeType(input, typeof(T));
				}
				catch
				{
					Console.WriteLine("Invalid input.");
					continue;
				}

				int errorIndex = GetBrokenRuleIndex(rules, result);

				if (errorIndex != -1)
				{
					Console.WriteLine(errorMsg[errorIndex]);
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
		/// Select requests for the index of given list.
		/// </summary>
		/// <param name="elements">List of elements to choose from.</param>
		/// <param name="toStrFunc">Function that describes how to format each element into a string. Note that the ordinal number is always inserted.</param>
		/// <param name="cancel">String upon whose input cancels the operation.</param>
		/// <returns>Index of the list `elements`.</returns>
		public static T Select(IList<T> elements, Func<T, string> toStrFunc, string cancel)
		{
			for (int i = 0; i < elements.Count; i++)
			{
				Console.WriteLine(i + ". " + toStrFunc.Invoke(elements[i]));
			}

			int selection = -1;

			try
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
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return elements[selection];
		}

		/// <summary>
		/// Select requests for the index of given list.
		/// </summary>
		/// <param name="elements">List of elements to choose from.</param>
		/// <param name="cancel">String upon whose input cancels the operation.</param>
		/// <returns>Index of the list `elements`.</returns>
		public static T Select(IList<T> elements, string cancel)
		{
			return Select(elements, (elem => elem.ToString()), cancel);
		}
	}
}
