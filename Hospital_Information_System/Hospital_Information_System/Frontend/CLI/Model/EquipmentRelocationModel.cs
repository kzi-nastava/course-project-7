using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;
using System.Diagnostics;
using HospitalIS.Backend.Controller;

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
		private const string errNoEquipmentAvailable = "There is no equipment available";

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

		private static void NewRelocation(string inputCancelString, Room newRoom = null)
		{
			var allProperties = EquipmentRelocationController.GetRelocationProperties();
			var relocation = newRoom != null ? InputDynamicRelocation(inputCancelString, newRoom) : InputRelocation(inputCancelString, allProperties);
			IS.Instance.EquipmentRelocationRepo.Add(relocation);
			EquipmentRelocationController.AddTask(relocation);
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


		private static Equipment InputChangeEquipment(string inputCancelString, EquipmentRelocation editingRelocation, bool dynamicRoom = false)
		{
			Debug.Assert(editingRelocation.RoomOld != null);

			return EasyInput<Equipment>.Select( !dynamicRoom ? RoomController.GetEquipment(editingRelocation.RoomOld) : RoomController.GetDynamicEquipment(editingRelocation.RoomOld),
				new List<Func<Equipment, bool>>(),
				new string[] { },
				eq => eq.ToString(),
				inputCancelString
			);
		}

		private static Room InputChangeRoomOld(string inputCancelString, EquipmentRelocation relocation, bool dynamicRoom = false)
		{
			return EasyInput<Room>.Select(
				RoomController.GetRooms().Where(room => EquipmentRelocationController.CanBeOldRoom(room, relocation, dynamicRoom)).ToList(),
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
		
		internal static void MoveDynamicEquipment(string inputCancelString)
		{
			try
			{
				List<Room> needsEquipment = GetRoomsThatNeedEquipment();

				Console.WriteLine("\n" + hintInputNewRoom);
				Room chosenRoom = EasyInput<Room>.Select(needsEquipment, inputCancelString);

				NewRelocation(inputCancelString, chosenRoom);
			}
			catch (NothingToSelectException)
			{
				Console.WriteLine(errNoEquipmentAvailable);
			}
			
		}

		private static List<Room> GetRoomsThatNeedEquipment()
		{
			List<Room> needsEquipment = new List<Room>();
			List<Room> rooms = RoomController.GetExeminationAndOperationRooms();	
			foreach (Room room in rooms)
			{
				List<Equipment> equipment = RoomController.GetDynamicEquipment(room);

				if (equipment.Count > 0)
				{
					PrintRoomsEquipment(room, equipment);
					needsEquipment.Add(room);
				}
			}

			return needsEquipment;
		}

		private static void PrintRoomsEquipment(Room room, List<Equipment> roomEquipment)
		{
			Console.WriteLine("\n" + room);
			
			foreach (Equipment equipment in roomEquipment)
			{
				if (RoomController.DoesNotHaveEquipmentRightNow(room, equipment))
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(equipment);
					Console.ResetColor();
					continue;
				}
				if (!RoomController.HasSomeEquipmentRightNow(room, equipment))
					Console.WriteLine(equipment);
			}
		}

		private static EquipmentRelocation InputDynamicRelocation(string inputCancelString, Room newRoom)
		{
			EquipmentRelocation result = new EquipmentRelocation();
			result.RoomNew = newRoom;
			
			Console.WriteLine(hintInputOldRoom);
			result.RoomOld = InputChangeRoomOld(inputCancelString, result, true);
			
			Console.WriteLine(hintInputEquipment);
			result.Equipment = InputChangeEquipment(inputCancelString, result, true);

			result.ScheduledFor = DateTime.Now;

			return result;
		}
	}
}
