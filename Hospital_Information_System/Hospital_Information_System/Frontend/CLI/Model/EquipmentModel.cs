using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;

namespace HospitalIS.Frontend.CLI.Model
{
	internal abstract class EquipmentModel
	{
		private static readonly string hintSearchSelectCriteria = "Select search criteria";
		private static readonly string hintSearch = "Enter search query";
		private static readonly string noResults = "No results";

		internal static void Search(Hospital hospital, string inputCancelString)
		{
			Console.WriteLine(hintSearchSelectCriteria);

			var matchBy = new Dictionary<string, Func<string, List<Equipment>>>
			{
				["Type"] = (string query) => MatchByType(hospital, query),
				["Use"] = (string query) => MatchByUse(hospital, query)
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

			if (searchResults.Count == 0)
			{
				Console.WriteLine(noResults);
			}
			else
			{
				foreach (var eq in searchResults)
				{
					// TODO @magley: Skip deleted rooms
					Console.WriteLine($"{eq} in room '{GetRoom(eq, hospital)?.Name}'");
				}
			}
		}
		private static List<Equipment> MatchByType(Hospital hospital, string searchQuery)
		{
			var result = new List<Equipment>();
			foreach (var equipment in hospital.Equipment)
			{
				var typeName = equipment.Type.ToString().ToLower();
				if (typeName.Contains(searchQuery.ToLower())) 
				{
					result.Add(equipment);
				}
			}
			return result;
		}

		private static List<Equipment> MatchByUse(Hospital hospital, string searchQuery)
		{
			var result = new List<Equipment>();
			foreach (var equipment in hospital.Equipment)
			{
				var useName = equipment.Use.ToString().ToLower();
				if (useName.Contains(searchQuery.ToLower()))
				{
					result.Add(equipment);
				}
			}
			return result;
		}

		private static Room GetRoom(Equipment equipment, Hospital hospital)
		{
			foreach (Room room in hospital.Rooms)
			{
				if (room.Equipment.Find(eq => eq.Id == equipment.Id) != null)
					return room;
			}
			return null; // Shouldn't ever happen
		}
	}
}
