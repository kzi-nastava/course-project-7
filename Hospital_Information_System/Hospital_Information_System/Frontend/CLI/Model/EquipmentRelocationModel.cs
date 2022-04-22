using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;

namespace HospitalIS.Frontend.CLI.Model
{
	internal static class EquipmentRelocationModel
	{
		private const string hintSelect = "Select action";
		private const string hintInputEquipment = "Select equipment to relocate";
		private const string hintInputOldRoom = "Select room from which to relocate the equipment";
		private const string hintInputNewRoom = "Select room to relocate the equipment in";
		private const string hintInputTimestamp = "Select date and time at which to perform the relocation";
		private const string hintSelectForDeletion = "Select scheduled relocation(s) to remove";

		internal static void Relocate(string inputCancelString)
		{
			var commandMap = new Dictionary<string, Action>
			{
				["New relocation"] = () => NewRelocation(inputCancelString),
				["List relocations"] = () => ListRelocations(),
				["Edit relocation"] = () => EditRelocation(inputCancelString),
				["Delete relocation"] = () => DeleteRelocation(inputCancelString),
			};

			Console.WriteLine(hintSelect);
			string selectedAction = EasyInput<string>.Select(commandMap.Keys.ToList(), inputCancelString);

			commandMap[selectedAction].Invoke();
		}

		private enum EquipmentRelocationProperty
		{
			EQUIPMENT,
			OLD_ROOM,
			NEW_ROOM,
			WHEN_TO_RELOCATE
		}

		private static void ListRelocations()
		{
			foreach (var relocation in IS.Instance.Hospital.EquipmentRelocations.OrderBy(rel => rel.ScheduledFor))
			{
				Console.WriteLine(relocation);
			}
		}

		private static void EditRelocation(string inputCancelString)
		{
			try
			{
				var selectedRelocation = EasyInput<EquipmentRelocation>.Select(IS.Instance.Hospital.EquipmentRelocations.ToList(), inputCancelString);
				var selectedProperties = EasyInput<EquipmentRelocationProperty>.SelectMultiple(
					Enum.GetValues(typeof(EquipmentRelocationProperty)).Cast<EquipmentRelocationProperty>().ToList(),
					inputCancelString
				).ToList();

				var inputResult = InputRelocation(inputCancelString, selectedProperties, selectedRelocation);
				Copy(selectedRelocation, inputResult, selectedProperties);
			}
			catch (InputCancelledException)
			{
			}
		}

		private static void NewRelocation(string inputCancelString)
		{
			var allRoomProperties = Enum.GetValues(typeof(EquipmentRelocationProperty)).Cast<EquipmentRelocationProperty>().ToList();
			try
			{
				var relocation = InputRelocation(inputCancelString, allRoomProperties, null);
				IS.Instance.EquipmentRelocationRepo.Add(relocation);
			}
			catch (InputCancelledException)
			{
			}
		}

		private static void DeleteRelocation(string inputCancelString)
		{
			Console.WriteLine(hintSelectForDeletion);

			try
			{
				var relocationsToDelete = EasyInput<EquipmentRelocation>.SelectMultiple(IS.Instance.Hospital.EquipmentRelocations.ToList(), inputCancelString);

				foreach (var relocation in relocationsToDelete)
				{
					IS.Instance.EquipmentRelocationRepo.Remove(relocation);
				}
			}
			catch (InputCancelledException)
			{
			}
		}

		private static EquipmentRelocation InputRelocation(string inputCancelString, List<EquipmentRelocationProperty> properties, EquipmentRelocation editingRelocation)
		{
			EquipmentRelocation temp = new EquipmentRelocation(); // We don't want to overwrite the relocation until the operation finishes
			EquipmentRelocation reference = editingRelocation ?? temp; // Read-only

			if (properties.Contains(EquipmentRelocationProperty.OLD_ROOM))
			{
				Console.WriteLine(hintInputOldRoom);
				temp.RoomOld = InputRelocationOldRoom(inputCancelString);
			}

			if (properties.Contains(EquipmentRelocationProperty.EQUIPMENT))
			{
				Console.WriteLine(hintInputEquipment);
				temp.Equipment = InputRelocationEquipment(inputCancelString, reference);
			}

			if (properties.Contains(EquipmentRelocationProperty.NEW_ROOM))
			{
				Console.WriteLine(hintInputNewRoom);
				temp.RoomNew = InputRelocationNewRoom(inputCancelString, reference);
			}

			if (properties.Contains(EquipmentRelocationProperty.WHEN_TO_RELOCATE))
			{
				Console.WriteLine(hintInputTimestamp);
				temp.ScheduledFor = InputRelocationTimestamp(inputCancelString);
			}

			return temp;
		}

		private static void Copy(EquipmentRelocation dest, EquipmentRelocation src, List<EquipmentRelocationProperty> properties)
		{
			if (properties.Contains(EquipmentRelocationProperty.EQUIPMENT)) dest.Equipment = src.Equipment;
			if (properties.Contains (EquipmentRelocationProperty.NEW_ROOM)) dest.RoomNew = src.RoomNew;
			if (properties.Contains(EquipmentRelocationProperty.WHEN_TO_RELOCATE)) dest.ScheduledFor = src.ScheduledFor;
		}

		private static Equipment InputRelocationEquipment(string inputCancelString, EquipmentRelocation editingRelocation)
		{
			// TODO @magley: Specify which ones are taken and overwrite

			return EasyInput<Equipment>.Select(
				IS.Instance.Hospital.Equipment.Where(eq => editingRelocation.RoomOld.Equipment.ContainsKey(eq)).ToList(),
				new List<Func<Equipment, bool>>()
				{
					eq => IS.Instance.Hospital.EquipmentRelocations.Where(
						eqRel => eqRel.Equipment == eq && (editingRelocation == null || editingRelocation != eqRel)
					).ToList().Count == 0,
				},
				new[]
				{
					"This equipment is already set for relocation"
				},
				eq => eq.ToString(),
				inputCancelString
			);
		}

		private static Room InputRelocationOldRoom(string inputCancelString)
		{
			return EasyInput<Room>.Select(
				IS.Instance.Hospital.Rooms.ToList(),
				new List<Func<Room, bool>>(),
				new string[] { },
				eq => eq.ToString(),
				inputCancelString
			);
		}

		private static Room InputRelocationNewRoom(string inputCancelString, EquipmentRelocation equipmentRelocation)
		{
			return EasyInput<Room>.Select(
				IS.Instance.Hospital.Rooms.ToList(),
				new List<Func<Room, bool>>()
				{
					room => equipmentRelocation == null || (equipmentRelocation.RoomOld != room)
				},
				new string[]
				{
					"This equipment already exists in that room",
				},
				room => room.ToString(),
				inputCancelString
			);
		}

		private static DateTime InputRelocationTimestamp(string inputCancelString)
		{
			return EasyInput<DateTime>.Get(new List<Func<DateTime, bool>>(), new string[] { }, inputCancelString);
		}
	}
}
