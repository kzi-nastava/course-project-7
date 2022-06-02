using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;
using System.Diagnostics;
using HospitalIS.Backend.Controller;
using HospitalIS.Frontend.CLI.View;

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
			string selectedCommand = EasyInput<string>.Select(commandMap.Keys.ToList(), inputCancelString);

			commandMap[selectedCommand].Invoke();
		}

		private static void ListRelocations()
		{
			foreach (var relocation in EquipmentRelocationController.GetRelocations().OrderBy(rel => rel.ScheduledFor))
			{
				Console.WriteLine(relocation);
			}
		}

		private static EquipmentRelocation SelectRelocation(string inputCancelString)
		{
			return EasyInput<EquipmentRelocation>.Select(EquipmentRelocationController.GetRelocations(), inputCancelString);
		}

		private static List<EquipmentRelocationController.Property> SelectEquipmentRelocationProperties(string inputCancelString)
		{
			return EasyInput<EquipmentRelocationController.Property>.SelectMultiple(EquipmentRelocationController.GetRelocationProperties(), inputCancelString).ToList();
		}

		private static void EditRelocation(string inputCancelString)
		{
			var selectedRelocation = SelectRelocation(inputCancelString);
			var selectedProperties = SelectEquipmentRelocationProperties(inputCancelString);
			var inputRelocation = InputRelocation(inputCancelString, selectedProperties, selectedRelocation);
			EquipmentRelocationController.Copy(selectedRelocation, inputRelocation, selectedProperties);
		}

		private static void NewRelocation(string inputCancelString)
		{
			var allProperties = EquipmentRelocationController.GetRelocationProperties();
			var relocation = InputRelocation(inputCancelString, allProperties);
			IS.Instance.EquipmentRelocationRepo.Add(relocation);
			IS.Instance.EquipmentRelocationRepo.AddTask(relocation);
		}

		private static void DeleteRelocation(string inputCancelString)
		{
			Console.WriteLine(hintSelectForDeletion);
			var relocationsToDelete = EasyInput<EquipmentRelocation>.SelectMultiple(EquipmentRelocationController.GetRelocations(), inputCancelString);

			foreach (var relocation in relocationsToDelete)
			{
				IS.Instance.EquipmentRelocationRepo.Remove(relocation);
			}
		}

		private static EquipmentRelocation InputRelocation(string inputCancelString, List<EquipmentRelocationController.Property> properties, EquipmentRelocation editingRelocation = null)
		{
			EquipmentRelocation result = new EquipmentRelocation();
			EquipmentRelocation reference = editingRelocation ?? result;

			Debug.Assert(reference != null);

			if (properties.Contains(EquipmentRelocationController.Property.OLD_ROOM))
			{
				Console.WriteLine(hintInputOldRoom);
				result.RoomOld = InputChangeRoomOld(inputCancelString, reference);
			}

			if (properties.Contains(EquipmentRelocationController.Property.EQUIPMENT))
			{
				Console.WriteLine(hintInputEquipment);
				result.Equipment = InputChangeEquipment(inputCancelString, reference);
			}

			if (properties.Contains(EquipmentRelocationController.Property.NEW_ROOM))
			{
				Console.WriteLine(hintInputNewRoom);
				result.RoomNew = InputRelocationNewRoom(inputCancelString);
			}

			if (properties.Contains(EquipmentRelocationController.Property.WHEN_TO_RELOCATE))
			{
				Console.WriteLine(hintInputTimestamp);
				result.ScheduledFor = InputChangeTimestamp(inputCancelString);
			}

			Debug.Assert(reference.ScheduledFor != DateTime.MinValue);

			return result;
		}


		private static Equipment InputChangeEquipment(string inputCancelString, EquipmentRelocation editingRelocation)
		{
			Debug.Assert(editingRelocation.RoomOld != null);

			return EasyInput<Equipment>.Select(
				RoomController.GetEquipment(editingRelocation.RoomOld),
				new List<Func<Equipment, bool>>(),
				new string[] { },
				eq => eq.ToString(),
				inputCancelString
			);
		}

		private static Room InputChangeRoomOld(string inputCancelString, EquipmentRelocation relocation)
		{
			return EasyInput<Room>.Select(
				RoomController.GetRooms().Where(room => EquipmentRelocationController.CanBeOldRoom(room, relocation)).ToList(),
				new List<Func<Room, bool>>(),
				new string[] { },
				eq => eq.ToString(),
				inputCancelString
			);
		}

		private static Room InputRelocationNewRoom(string inputCancelString)
		{
			return EasyInput<Room>.Select(
				RoomController.GetRooms(),
				new List<Func<Room, bool>>(),
				new string[] { },
				room => room.ToString(),
				inputCancelString
			);
		}

		private static DateTime InputChangeTimestamp(string inputCancelString)
		{
			return EasyInput<DateTime>.Get(new List<Func<DateTime, bool>>(), new string[] { }, inputCancelString);
		}
	}
}
