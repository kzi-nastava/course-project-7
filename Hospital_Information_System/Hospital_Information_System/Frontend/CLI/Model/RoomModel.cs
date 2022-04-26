using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;

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

		internal static void DeleteRoom(string inputCancelString)
		{
			Console.WriteLine(hintSelectRooms);

			try
			{
				var roomsToDelete = EasyInput<Room>.SelectMultiple(GetModifiableRooms(), r => r.Name, inputCancelString);
				foreach (var room in roomsToDelete)
				{
					IS.Instance.RoomRepo.Remove(room);
				}
			}
			catch (InputCancelledException)
			{
			}
		}

		internal static void CreateRoom(string inputCancelString)
		{
			var roomPropertiesAll = Enum.GetValues(typeof(RoomProperty)).Cast<RoomProperty>().ToList();
			try
			{
				Room room = InputRoom(inputCancelString, roomPropertiesAll);
				IS.Instance.RoomRepo.Add(room);
			}
			catch (InputCancelledException)
			{
			}
		}

		internal static void UpdateRoom(string inputCancelString)
		{
			try
			{
				Console.WriteLine(hintSelectRoom);
				Room room = EasyInput<Room>.Select(GetModifiableRooms(), r => r.Name, inputCancelString);
				Console.WriteLine(room.ToString());

				Console.WriteLine(hintSelectProperties);
				var propertiesToUpdate = EasyInput<RoomProperty>.SelectMultiple(
					Enum.GetValues(typeof(RoomProperty)).Cast<RoomProperty>().ToList(),
					e => Enum.GetName(typeof(RoomProperty), e),
					inputCancelString
				).ToList();

				var updatedRoom = InputRoom(inputCancelString, propertiesToUpdate);
				CopyRoom(room, updatedRoom, propertiesToUpdate);
			}
			catch (InputCancelledException)
			{
			}
		}

		internal enum RoomProperty
		{
			NAME,
			FLOOR,
			TYPE,
		}

		internal static Room InputRoom(string inputCancelString, List<RoomProperty> whichProperties)
		{
			Room room = new Room();

			if (whichProperties.Contains(RoomProperty.NAME))
			{
				Console.WriteLine(hintRoomName);
				room.Name = InputRoomName(inputCancelString);
			}

			if (whichProperties.Contains(RoomProperty.FLOOR))
			{
				Console.WriteLine(hintRoomFloor);
				room.Floor = InputRoomFloor(inputCancelString);
			}

			if (whichProperties.Contains(RoomProperty.TYPE))
			{
				Console.WriteLine(hintRoomType);
				room.Type = InputRoomType(inputCancelString);
			}

			return room;
		}

		private static List<Room> GetModifiableRooms()
		{
			return IS.Instance.Hospital.Rooms.Where(r => !r.Deleted && r.Type != Room.RoomType.WAREHOUSE).ToList();
		}

		private static void CopyRoom(Room target, Room source, List<RoomProperty> whichProperties)
		{
			if (whichProperties.Contains(RoomProperty.NAME)) target.Name = source.Name;
			if (whichProperties.Contains(RoomProperty.FLOOR)) target.Floor = source.Floor;
			if (whichProperties.Contains(RoomProperty.TYPE)) target.Type = source.Type;
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
				Enum.GetValues(typeof(Room.RoomType)).Cast<Room.RoomType>()
					.Where(e => e != Room.RoomType.WAREHOUSE)
					.ToList(),
				inputCancelString
			);
		}
	}
}