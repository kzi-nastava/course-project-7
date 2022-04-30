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

			var matchBy = new Dictionary<string, Func<string, List<Equipment>>>
			{
				["Type"] = (string query) => EquipmentController.MatchByType(query),
				["Use"] = (string query) => EquipmentController.MatchByUse(query)
			};
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
	}
}
