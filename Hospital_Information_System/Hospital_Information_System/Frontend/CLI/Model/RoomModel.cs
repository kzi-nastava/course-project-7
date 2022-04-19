using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using System.Linq;

namespace HospitalIS.Frontend.CLI.Model
{
	internal abstract class RoomModel
	{
		private static readonly string hintSelectRooms = "Select rooms by their number, separated by whitespace.\nEnter a newline to finish";
		private static readonly string hintSelectRoom = "Select room";
		private static readonly string hintSelectProperties = "Select properties by their number, separated by whitespace.\nEnter a newline to finish";
		private static readonly string hintRoomName = "Enter room name";
		private static readonly string hintRoomFloor = "Enter room floor";
		private static readonly string hintRoomType = "Select room type";
		private static readonly string hintRoomNameNonEmpty = "Room name must not be empty!";
		private static readonly string hintRoomFloorNonNegative = "Room floor must be a non-negative integer!";
		internal static void DeleteRoom(Hospital hospital, string inputCancelString)
		{
			Console.WriteLine(hintSelectRooms);

			try
			{
				var roomsToDelete = EasyInput<Room>.SelectMultiple(GetModifiableRooms(hospital), r => r.Name, inputCancelString);
				foreach (var room in roomsToDelete)
				{
					room.Deleted = true;
					hospital.GetWarehouse().Equipment.AddRange(room.Equipment);
					room.Equipment.Clear();
				}
			}
			catch (InputCancelledException)
			{
			}
		}
		internal static void CreateRoom(Hospital hospital, string inputCancelString)
		{
			var allRoomProperties = Enum.GetValues(typeof(RoomProperty)).Cast<RoomProperty>().ToList();
			try
			{
				Room room = InputRoom(hospital, inputCancelString, allRoomProperties);
				hospital.Add(room);
			}
			catch (InputCancelledException)
			{
			}
		}
		internal static void UpdateRoom(Hospital hospital, string inputCancelString)
		{
			try
			{
				Console.WriteLine(hintSelectRoom);
				Room room = EasyInput<Room>.Select(GetModifiableRooms(hospital), r => r.Name, inputCancelString);
				Console.WriteLine(room.ToString());
				Console.WriteLine(hintSelectProperties);

				var propertiesToUpdate = EasyInput<RoomProperty>.SelectMultiple(
					Enum.GetValues(typeof(RoomProperty)).Cast<RoomProperty>().ToList(),
					e => Enum.GetName(typeof(RoomProperty), e),
					inputCancelString
				).ToList();

				var updatedRoom = InputRoom(hospital, inputCancelString, propertiesToUpdate);
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
		internal static Room InputRoom(Hospital hospital, string inputCancelString, List<RoomProperty> whichProperties)
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
				room.Floor = InputRoomFloor(inputCancelString, room);
			}

			if (whichProperties.Contains(RoomProperty.TYPE))
			{
				Console.WriteLine(hintRoomType);
				room.Type = InputRoomType(inputCancelString, room);
			}

			return room;
		}
		private static List<Room> GetModifiableRooms(Hospital hospital)
		{
			return hospital.Rooms.Where(r => !r.Deleted && r.Type != Room.RoomType.WAREHOUSE).ToList();
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
				new List<Func<string, bool>> { s => s.Count() != 0 },
				new[] { hintRoomNameNonEmpty },
				inputCancelString
			);
		}
		private static int InputRoomFloor(string inputCancelString, Room room)
		{
			return EasyInput<int>.Get(
				new List<Func<int, bool>> { n => n >= 0 },
				new[] { hintRoomFloorNonNegative },
				inputCancelString
			);
		}
		private static Room.RoomType InputRoomType(string inputCancelString, Room room)
		{
			return EasyInput<Room.RoomType>.Select(
				Enum.GetValues(typeof(Room.RoomType)).Cast<Room.RoomType>().Where(e => e != Room.RoomType.WAREHOUSE).ToList(),
				inputCancelString
			);
		}
	}
}