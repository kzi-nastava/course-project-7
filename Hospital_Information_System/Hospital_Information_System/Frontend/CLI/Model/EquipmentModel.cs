using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;

namespace HospitalIS.Frontend.CLI.Model
{
	internal abstract class EquipmentModel
	{
		private const string hintSearchSelectCriteria = "Select search criteria";
		private const string hintSearch = "Enter search query";
		private const string noResults = "No results";
		private const string hintFilter = "Select criterion to filter by";
		private const string hintFilterByType = "Select type of equipment";
		private const string hintFilterByUse = "Select use of equipment";

		private static Equipment.EquipmentType SelectType(string inputCancelString)
		{
			Console.WriteLine(hintFilterByType);
			return EasyInput<Equipment.EquipmentType>.Select(
				Enum.GetValues(typeof(Equipment.EquipmentType)).Cast<Equipment.EquipmentType>().ToList(),
				inputCancelString
			);
		}

		private static Equipment.EquipmentUse SelectUse(string inputCancelString)
		{
			Console.WriteLine(hintFilterByUse);
			return EasyInput<Equipment.EquipmentUse>.Select(
				Enum.GetValues(typeof(Equipment.EquipmentUse)).Cast<Equipment.EquipmentUse>().ToList(),
				inputCancelString
			);
		}

		internal static void Filter(string inputCancelString)
		{
			var filterBy = new Dictionary<string, Func<List<Equipment>>>
			{
				["Filter by type"] = () => FilterByType(SelectType(inputCancelString)),
				["Filter by use"] = () => FilterByUse(SelectUse(inputCancelString)),
				["Filter by out of stock"] = () => FilterByAmount(num => num == 0),
				["Filter by less than 10"] = () => FilterByAmount(num => num >= 0 && num < 10),
				["Filter by more than 10"] = () => FilterByAmount(num => num >= 10),
			};

			Console.WriteLine(hintFilter);
			var filterChoice = EasyInput<string>.Select(filterBy.Keys.ToList(), inputCancelString);

			var filterResults = filterBy[filterChoice]();
			PrintResults(filterResults, eq => $"{eq} [total items: {GetTotalCount(eq)}]");
		}

		private static List<Equipment> FilterByAmount(Func<int, bool> amountPredicate)
		{
			return (from eq 
					in IS.Instance.Hospital.Equipment 
					where amountPredicate(GetTotalCount(eq)) 
					select eq
			).ToList();
		}

		private static List<Equipment> FilterByUse(Equipment.EquipmentUse use)
		{
			return IS.Instance.Hospital.Equipment.Where(eq => eq.Use == use).ToList();
		}

		private static List<Equipment> FilterByType(Equipment.EquipmentType type)
		{
			return IS.Instance.Hospital.Equipment.Where(eq => eq.Type == type).ToList();
		}

		private static int GetTotalCount(Equipment equipment)
		{
			int numberOfContainingRooms = 0;
			IS.Instance.Hospital.Rooms.ForEach(r => { if (r.Equipment.ContainsKey(equipment)) numberOfContainingRooms += r.Equipment[equipment]; });
			return numberOfContainingRooms;
		}

		private static int GetRoomCount(Equipment equipment)
		{
			int numberOfContainingRooms = 0;
			IS.Instance.Hospital.Rooms.ForEach(r => { if (r.Equipment.ContainsKey(equipment)) numberOfContainingRooms++; });
			return numberOfContainingRooms;
		}

		internal static void Search(string inputCancelString)
		{
			Console.WriteLine(hintSearchSelectCriteria);

			var matchBy = new Dictionary<string, Func<string, List<Equipment>>>
			{
				["Type"] = (string query) => MatchByType(query),
				["Use"] = (string query) => MatchByUse(query)
			};
			var critsSelected = EasyInput<string>.SelectMultiple(matchBy.Keys.ToList(), inputCancelString);

			Console.WriteLine(hintSearch);
			string query = Console.ReadLine();

			var searchResults = new List<Equipment>();
			foreach (string criterion in critsSelected)
			{
				searchResults.AddRange(matchBy[criterion](query));
			}

			// TODO @magley: Consider using a HashSet here
			searchResults = searchResults.GroupBy(e => e.Id).Select(grp => grp.First()).ToList();
			PrintResults(searchResults, eq => PrintSingleEquipment(eq));
		}

		private static string PrintSingleEquipment(Equipment equipment)
		{
			int containingRoomCount = GetRoomCount(equipment);
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

		private static List<Equipment> MatchByType(string searchQuery)
		{
			var matchingEquipment = new List<Equipment>();
			foreach (var equipment in IS.Instance.Hospital.Equipment)
			{
				var typeName = equipment.Type.ToString().ToLower();
				if (typeName.Contains(searchQuery.ToLower()))
				{
					matchingEquipment.Add(equipment);
				}
			}
			return matchingEquipment;
		}

		private static List<Equipment> MatchByUse(string searchQuery)
		{
			var matchingEquipment = new List<Equipment>();
			foreach (var equipment in IS.Instance.Hospital.Equipment)
			{
				var useName = equipment.Use.ToString().ToLower();
				if (useName.Contains(searchQuery.ToLower()))
				{
					matchingEquipment.Add(equipment);
				}
			}
			return matchingEquipment;
		}
	}
}
