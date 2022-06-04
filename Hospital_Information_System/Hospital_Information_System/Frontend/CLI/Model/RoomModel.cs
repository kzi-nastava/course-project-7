using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
	internal abstract class RoomModel
	{
		private const string hintSelectRooms = "Select rooms by their number, separated by whitespace.\nEnter a newline to finish";
		private const string hintSelectRoom = "Select room, by its number.\nEnter a newline to finish";
		private const string hintSelectProperties = "Select properties by their number, separated by whitespace.\nEnter a newline to finish";
		private const string hintRoomName = "Enter room name";
		private const string hintRoomFloor = "Enter room floor";
		private const string hintRoomType = "Select room type";
		private const string errRoomNameNonEmpty = "Room name must not be empty!";
		private const string errRoomFloorNonNegative = "Room floor must be a non-negative integer!";

		internal static void CreateRoom(string inputCancelString)
		{
			Room room = InputRoom(inputCancelString, RoomController.GetRoomProperties());
			IS.Instance.RoomRepo.Add(room);
		}

		internal static void ViewRoom(string inputCancelString)
		{
			Console.WriteLine(hintSelectRoom);
			var room = EasyInput<Room>.Select(RoomController.GetModifiableRooms(), r => r.Name, inputCancelString);
			Console.WriteLine(room.ToString());
		}

		internal static void UpdateRoom(string inputCancelString)
		{
			Console.WriteLine(hintSelectRoom);
			var room = EasyInput<Room>.Select(RoomController.GetModifiableRooms(), r => r.Name, inputCancelString);
			Console.WriteLine(room.ToString());

			Console.WriteLine(hintSelectProperties);
			var propertiesToUpdate = SelectRoomProperties(inputCancelString);
			var updatedRoom = InputRoom(inputCancelString, propertiesToUpdate);
			RoomController.CopyRoom(room, updatedRoom, propertiesToUpdate);
		}

		internal static void DeleteRoom(string inputCancelString)
		{
			Console.WriteLine(hintSelectRooms);

			var selectedRoomsForDeletion = SelectModifiableRooms(inputCancelString);
			foreach (var room in selectedRoomsForDeletion)
			{
				IS.Instance.RoomRepo.Remove(room);
			}
		}

		private static List<Room> SelectModifiableRooms(string inputCancelString)
		{
			return EasyInput<Room>.SelectMultiple(RoomController.GetModifiableRooms(), r => r.Name, inputCancelString).ToList();
		}

		private static List<RoomController.RoomProperty> SelectRoomProperties(string inputCancelString)
		{
			return EasyInput<RoomController.RoomProperty>.SelectMultiple(
				RoomController.GetRoomProperties(),
				e => Enum.GetName(typeof(RoomController.RoomProperty), e),
				inputCancelString
			).ToList();
		}

		public static Room InputRoom(string inputCancelString, List<RoomController.RoomProperty> whichProperties)
		{
			Room room = new Room();

			if (whichProperties.Contains(RoomController.RoomProperty.NAME))
			{
				Console.WriteLine(hintRoomName);
				room.Name = InputRoomName(inputCancelString);
			}

			if (whichProperties.Contains(RoomController.RoomProperty.FLOOR))
			{
				Console.WriteLine(hintRoomFloor);
				room.Floor = InputRoomFloor(inputCancelString);
			}

			if (whichProperties.Contains(RoomController.RoomProperty.TYPE))
			{
				Console.WriteLine(hintRoomType);
				room.Type = InputRoomType(inputCancelString);
			}

			return room;
		}

		private static string InputRoomName(string inputCancelString)
		{
			return EasyInput<string>.Get(
				new List<Func<string, bool>> { s => s.Length != 0 },
				new[] { errRoomNameNonEmpty },
				inputCancelString
			);
		}

		private static int InputRoomFloor(string inputCancelString)
		{
			return EasyInput<int>.Get(
				new List<Func<int, bool>> { n => n >= 0 },
				new[] { errRoomFloorNonNegative },
				inputCancelString
			);
		}
		private static Room.RoomType InputRoomType(string inputCancelString)
		{
			return EasyInput<Room.RoomType>.Select(
				RoomController.GetModifiableRoomTypes(),
				inputCancelString
			);
		}
	}
}