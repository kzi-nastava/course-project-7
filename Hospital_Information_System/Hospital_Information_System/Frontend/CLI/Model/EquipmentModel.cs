using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
	internal abstract class EquipmentModel
	{
		private const string hintSearchSelectCriteria = "Select criteria to search by, separated by whitespace.\nEnter a newline to finish";
		private const string hintSearch = "Enter search query";
		private const string noResults = "No results";
		private const string hintFilter = "Select criteria to filer by, separated by whitespace.\nEnter a newline to finish";
		private const string hintFilterByType = "Select type of equipment";
		private const string hintFilterByUse = "Select use of equipment";
		private const string errNotEnoughEquipment = "This room does not have that much equipment";
		private const string showCurrentState = "Current state in the room [Equipment: Quantity]:";
		private const string errNoEquipmentInRoom = "There's no equipment in this room";
		private const string errPositiveNumber = "You must input a positive number (or 0)";
		private const string hintInputUsedEquipment = "For each equipment, input how many you have used: ";
		
		private static readonly Dictionary<string, Func<string, List<Equipment>>> matchBy = new Dictionary<string, Func<string, List<Equipment>>>
		{
			["Type"] = (string query) => EquipmentController.MatchByType(query),
			["Use"] = (string query) => EquipmentController.MatchByUse(query)
		};

		internal static void Filter(string inputCancelString)
		{
			var filterBy = new Dictionary<string, Func<List<Equipment>>>
			{
				["Filter by type"] = () => EquipmentController.FilterByType(SelectType(inputCancelString)),
				["Filter by use"] = () => EquipmentController.FilterByUse(SelectUse(inputCancelString)),
				["Filter by out of stock"] = () => EquipmentController.FilterByAmount(num => num == 0),
				["Filter by less than 10"] = () => EquipmentController.FilterByAmount(num => num >= 0 && num < 10),
				["Filter by more than 10"] = () => EquipmentController.FilterByAmount(num => num >= 10),
			};

			Console.WriteLine(hintFilter);
			var filterChoice = EasyInput<string>.Select(filterBy.Keys.ToList(), inputCancelString);

			var filterResults = filterBy[filterChoice]();
			PrintResults(filterResults, eq => $"{eq} [total items: {EquipmentController.GetTotalSupplyCount(eq)}]");
		}

		internal static void Search(string inputCancelString)
		{
			Console.WriteLine(hintSearchSelectCriteria);
			var critsSelected = EasyInput<string>.SelectMultiple(matchBy.Keys.ToList(), inputCancelString);

			Console.WriteLine(hintSearch);
			string searchQuery = Console.ReadLine();

			var searchResults = new List<Equipment>();
			foreach (string criterion in critsSelected)
			{
				searchResults.AddRange(matchBy[criterion](searchQuery));
			}

			searchResults = searchResults.Distinct().ToList();
			PrintResults(searchResults, eq => PrintSingleEquipment(eq));
		}

		private static Equipment.EquipmentType SelectType(string inputCancelString)
		{
			Console.WriteLine(hintFilterByType);
			return EasyInput<Equipment.EquipmentType>.Select(
				EquipmentController.GetEquipmentTypes(),
				inputCancelString
			);
		}

		private static Equipment.EquipmentUse SelectUse(string inputCancelString)
		{
			Console.WriteLine(hintFilterByUse);
			return EasyInput<Equipment.EquipmentUse>.Select(
				EquipmentController.GetEquipmentUses(),
				inputCancelString
			);
		}


		private static string PrintSingleEquipment(Equipment equipment)
		{
			int containingRoomCount = EquipmentController.GetContainingRoomCount(equipment);
			return $"{equipment} in {containingRoomCount} room{(containingRoomCount != 1 ? "s" : "")}";
		}

		private static void PrintResults(List<Equipment> results, Func<Equipment, string> toStringFunc)
		{
			if (results.Count == 0)
			{
				Console.WriteLine(noResults);
			}
			else
			{
				foreach (var eq in results)
				{
					Console.WriteLine($"{toStringFunc(eq)}");
				}
			}
		}
		

		public static void DeleteEquipmentAfterAppointment(Room room, string inputCancelString)
		{
			Dictionary<Equipment, int> currentEquipmentQuantity = room.Equipment;
			if (currentEquipmentQuantity.Count == 0)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(errNoEquipmentInRoom);
				Console.ForegroundColor = ConsoleColor.Gray;
				return;
			}
			Print(currentEquipmentQuantity);
			var newEquipmentQuantity = GetNewEquipmentQuantity(inputCancelString, currentEquipmentQuantity);
			room.Equipment = newEquipmentQuantity;
		}
		
		private static void Print(Dictionary<Equipment, int> currentEquipmentQuantity)
		{
			Console.WriteLine(showCurrentState);
			foreach (KeyValuePair<Equipment, int> entry in currentEquipmentQuantity)
			{
				Console.WriteLine(entry.Key + ": " + entry.Value);
			}
			
		}
		
		private static Dictionary<Equipment, int> GetNewEquipmentQuantity(string inputCancelString,
			Dictionary<Equipment, int> oldEquipmentQuantity)
		{
			Dictionary<Equipment, int> newEquipmentQuantity = new Dictionary<Equipment, int>();
			Console.WriteLine(hintInputUsedEquipment);
			foreach (KeyValuePair<Equipment, int> entry in oldEquipmentQuantity)
			{
				var equipment = entry.Key;
				Console.Write(equipment + ": ");
				int currentQuantity = entry.Value;
				int usedQuantity = GetUsedEquipmentQuantity(inputCancelString, currentQuantity);
				int newQuantity = currentQuantity - usedQuantity;
				if (newQuantity != 0)
				{
					newEquipmentQuantity[equipment] = newQuantity;
				}
			}

			return newEquipmentQuantity;
		}
		
		private static int GetUsedEquipmentQuantity(string inputCancelString, int currentQuantity)
		{
			return EasyInput<int>.Get(
				new List<Func<int, bool>>
				{
					s => s <= currentQuantity,
					s => s >= 0,
				},
				new[]
				{
					errNotEnoughEquipment,
					errPositiveNumber,
				},
				inputCancelString
			);
		}
	}
}
