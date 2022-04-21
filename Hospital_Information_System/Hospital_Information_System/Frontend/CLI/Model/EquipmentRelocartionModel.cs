using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using HospitalIS.Backend.Repository;
using System.Linq;


namespace HospitalIS.Frontend.CLI.Model
{
	internal class EquipmentRelocartionModel
	{
		private static readonly string hintSelect = "Select action";
		private static readonly string hintInputEquipment = "Select equipment to relocate";
		private static readonly string hintInputNewRoom = "Select room to relocate the equipment in";
		private static readonly string hintInputTimestamp = "Select date and time at which to perform the relocation";
		private static readonly string hintSelectForDeletion = "Select scheduled relocation(s) to remove";

		internal static void Relocate(Hospital hospital, string inputCancelString)
		{
			var commandMap = new Dictionary<string, Action>
			{
				["New relocation"] = () => NewRelocation(hospital, inputCancelString),
				["List relocations"] = () => ListRelocations(hospital, inputCancelString),
				["Edit relocation"] = () => EditRelocation(hospital, inputCancelString),
				["Delete relocation"] = () => DeleteRelocation(hospital, inputCancelString),
			};

			Console.WriteLine(hintSelect);
			string selectedAction = EasyInput<string>.Select(commandMap.Keys.ToList(), inputCancelString);

			commandMap[selectedAction].Invoke();
		}

		private enum EquipmentRelocationProperty
		{
			EQUIPMENT,
			NEW_ROOM,
			WHEN_TO_RELOCATE
		}

		private static void ListRelocations(Hospital hospital, string inputCancelString)
		{
			var relocationsSorted = hospital.EquipmentRelocations.OrderBy(rel => rel.ScheduledFor);
			foreach (var relocation in relocationsSorted)
			{
				Console.WriteLine(relocation);
			}
		}

		private static void EditRelocation(Hospital hospital, string inputCancelString)
		{
			try
			{
				var selectedRelocation = EasyInput<EquipmentRelocation>.Select(hospital.EquipmentRelocations.ToList(), inputCancelString);
				var selectedProperties = EasyInput<EquipmentRelocationProperty>.SelectMultiple(
					Enum.GetValues(typeof(EquipmentRelocationProperty)).Cast<EquipmentRelocationProperty>().ToList(),
					inputCancelString
				).ToList();

				var inputResult = InputRelocation(hospital, inputCancelString, selectedProperties, selectedRelocation);
				Copy(selectedRelocation, inputResult, selectedProperties);
			}
			catch (InputCancelledException)
			{
			}
		}

		private static void NewRelocation(Hospital hospital, string inputCancelString)
		{
			var allRoomProperties = Enum.GetValues(typeof(EquipmentRelocationProperty)).Cast<EquipmentRelocationProperty>().ToList();
			try
			{
				var relocation = InputRelocation(hospital, inputCancelString, allRoomProperties, null);
				hospital.Add(relocation);
			}
			catch (InputCancelledException)
			{
			}
		}

		private static void DeleteRelocation(Hospital hospital, string inputCancelString)
		{
			Console.WriteLine(hintSelectForDeletion);

			try
			{
				var relocationsToDelete = EasyInput<EquipmentRelocation>.SelectMultiple(hospital.EquipmentRelocations.ToList(), inputCancelString);

				foreach (var relocation in relocationsToDelete)
				{
					relocation.Deleted = true;
				}
			}
			catch (InputCancelledException)
			{
			}
		}

		private static EquipmentRelocation InputRelocation(Hospital hospital, string inputCancelString, List<EquipmentRelocationProperty> properties, EquipmentRelocation editingRelocation)
		{
			EquipmentRelocation temp = new EquipmentRelocation();
			EquipmentRelocation reference = editingRelocation ?? temp;

			if (properties.Contains(EquipmentRelocationProperty.EQUIPMENT))
			{
				Console.WriteLine(hintInputEquipment);
				temp.Equipment = InputRelocationEquipment(hospital, inputCancelString, reference);
			}

			if (properties.Contains(EquipmentRelocationProperty.NEW_ROOM))
			{
				Console.WriteLine(hintInputNewRoom);
				temp.RoomNew = InputRelocationNewRoom(hospital, inputCancelString, reference);
			}

			if (properties.Contains(EquipmentRelocationProperty.WHEN_TO_RELOCATE))
			{
				Console.WriteLine(hintInputTimestamp);
				temp.ScheduledFor = InputRelocationTimestamp(hospital, inputCancelString, reference);
			}

			return temp;
		}

		private static void Copy(EquipmentRelocation dest, EquipmentRelocation src, List<EquipmentRelocationProperty> properties)
		{
			if (properties.Contains(EquipmentRelocationProperty.EQUIPMENT)) dest.Equipment = src.Equipment;
			if (properties.Contains (EquipmentRelocationProperty.NEW_ROOM)) dest.RoomNew = src.RoomNew;
			if (properties.Contains(EquipmentRelocationProperty.WHEN_TO_RELOCATE)) dest.ScheduledFor = src.ScheduledFor;
		}

		private static Equipment InputRelocationEquipment(Hospital hospital, string inputCancelString, EquipmentRelocation editingRelocation)
		{
			// TODO @magley: Specify which ones are taken and overwrite
			return EasyInput<Equipment>.Select(
				hospital.Equipment.ToList(),
				new List<Func<Equipment, bool>>()
				{
					eq => hospital.EquipmentRelocations.Where(eqRel => eqRel.Equipment == eq && (editingRelocation == null || editingRelocation != eqRel)).ToList().Count == 0,
				},
				new[]
				{
					"This equipment is already set for relocation"
				},
				eq => eq.ToString(),
				inputCancelString
			);
		}

		private static Room InputRelocationNewRoom(Hospital hospital, string inputCancelString, EquipmentRelocation equipmentRelocation)
		{
			return EasyInput<Room>.Select(
				hospital.Rooms.ToList(),
				new List<Func<Room, bool>>()
				{
					rm => equipmentRelocation == null || (RoomHasEquipmentRepository.GetRoom(hospital, equipmentRelocation.Equipment) != rm),
					rm => equipmentRelocation == null || (equipmentRelocation.RoomNew != rm)
				},
				new[]
				{
					"This equipment is already in that room",
					"This equipment is already scheduled to go into that room",
				},
				eq => eq.ToString(),
				inputCancelString);
		}

		private static DateTime InputRelocationTimestamp(Hospital hospital, string inputCancelString, EquipmentRelocation equipmentRelocation)
		{
			return EasyInput<DateTime>.Get(new List<Func<DateTime, bool>>(), new string[] { }, inputCancelString);
		}
	}
}
