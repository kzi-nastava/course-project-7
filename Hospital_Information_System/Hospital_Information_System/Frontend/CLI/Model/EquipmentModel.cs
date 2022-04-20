using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;

namespace HospitalIS.Frontend.CLI.Model
{
	internal class DanglingEquipmentException : Exception
	{
	}
	internal abstract class EquipmentModel
	{
		private static readonly string hintSearchSelectCriteria = "Select search criteria";
		private static readonly string hintSearch = "Enter search query";
		private static readonly string noResults = "No results";
		private static readonly string hintFilter = "Select criterion to filter by";
		private static readonly string hintFilterByType = "Select type of equipment";
		private static readonly string hintFilterByUse = "Select use of equipment";

		internal static void Filter(Hospital hospital, string inputCancelString)
		{		
			Equipment.EquipmentType selectType()
			{
				Console.WriteLine(hintFilterByType);
				return EasyInput<Equipment.EquipmentType>.Select(
					Enum.GetValues(typeof(Equipment.EquipmentType)).Cast<Equipment.EquipmentType>().ToList(),
					inputCancelString
				);
			}

			Equipment.EquipmentUse selectUse()
			{
				Console.WriteLine(hintFilterByUse);
				return EasyInput<Equipment.EquipmentUse>.Select(
					Enum.GetValues(typeof(Equipment.EquipmentUse)).Cast<Equipment.EquipmentUse>().ToList(),
					inputCancelString
				);
			}

			var filterBy = new Dictionary<string, Func<List<Equipment>>>
			{
				["Filter by type"] = () => FilterByType(hospital, selectType()),
				["Filter by use"] = () => FilterByUse(hospital, selectUse()),
				["Filter by out of stock"] = () => FilterByAmountZero(hospital),
				["Filter by less than 10"] = () => FilterByAmountLessThan10(hospital),
				["Filter by more than 10"] = () => FilterByAmountMoreThan10(hospital),
			};

			Console.WriteLine(hintFilter);
			var filterChoice = EasyInput<string>.Select(filterBy.Keys.ToList(), inputCancelString);

			var filterResults = filterBy[filterChoice]();
			PrintResults(hospital, filterResults);
		}

		private static List<Equipment> FilterByAmountZero(Hospital hospital)
		{
			var typesOutOfStock = Enum.GetValues(typeof(Equipment.EquipmentType))
				.Cast<Equipment.EquipmentType>()
				.Where(x => (hospital.Equipment.Where(eq => eq.Type == x).Count() == 0))
				.Select(x => x)
				.ToList();

			var result = new List<Equipment>();
			foreach (var type in typesOutOfStock)
			{
				result.Add(new Equipment(type, Equipment.EquipmentUse.Unknown));
			}
			return result;
		}
		private static List<Equipment> FilterByAmountMoreThan10(Hospital hospital)
		{
			var typesMoreThan10 = hospital.Equipment
				.GroupBy(eq => eq.Type)
				.Where(group => group.Count() > 10)
				.Select(eq => eq.Key)
				.ToList();
			return hospital.Equipment
				.Where(eq => typesMoreThan10.Contains(eq.Type))
				.Select(eq => eq)
				.OrderBy(eq => eq.Type.ToString())
				.ToList();
		}
		private static List<Equipment> FilterByAmountLessThan10(Hospital hospital)
		{
			var typesLessThan10 = hospital.Equipment
				.GroupBy(eq => eq.Type)
				.Where(group => group.Count() > 0 && group.Count() < 10)
				.Select(eq => eq.Key)
				.ToList();

			return hospital.Equipment
				.Where(eq => typesLessThan10.Contains(eq.Type))
				.Select(eq => eq)
				.OrderBy(eq => eq.Type.ToString())
				.ToList();
		}

		private static List<Equipment> FilterByUse(Hospital hospital, Equipment.EquipmentUse use)
		{
			return hospital.Equipment.Where(eq => eq.Use == use).ToList();
		}

		private static List<Equipment> FilterByType(Hospital hospital, Equipment.EquipmentType type)
		{
			return hospital.Equipment.Where(eq => eq.Type == type).ToList();
		}

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
			PrintResults(hospital, searchResults);
		}

		private static void PrintResults(Hospital hospital, List<Equipment> results)
		{
			if (results.Count == 0)
			{
				Console.WriteLine(noResults);
			}
			else
			{
				foreach (var eq in results)
				{
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

		internal static Room GetRoom(Equipment equipment, Hospital hospital)
		{
			if (equipment.Id == -1)
				return null;

			foreach (Room room in hospital.Rooms)
			{
				if (room.Equipment.Find(eq => eq.Id == equipment.Id) != null)
					return room;
			}
			throw new DanglingEquipmentException();
		}
	}
}
