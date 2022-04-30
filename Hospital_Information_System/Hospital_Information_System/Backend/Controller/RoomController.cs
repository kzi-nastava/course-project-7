using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Controller
{
	internal class RoomController
	{
		public enum RoomProperty
		{
			NAME,
			FLOOR,
			TYPE
		}

		public static List<RoomProperty> GetRoomProperties()
		{
			return Enum.GetValues(typeof(RoomProperty)).Cast<RoomProperty>().ToList();
		}

		public static void CopyRoom(Room target, Room source, List<RoomProperty> whichProperties)
		{
			if (whichProperties.Contains(RoomProperty.NAME)) target.Name = source.Name;
			if (whichProperties.Contains(RoomProperty.FLOOR)) target.Floor = source.Floor;
			if (whichProperties.Contains(RoomProperty.TYPE)) target.Type = source.Type;
		}

		public static List<Room> GetModifiableRooms()
		{
			return GetRooms().Where(r => r.Type != Room.RoomType.WAREHOUSE).ToList();
		}

		public static List<Room> GetRooms()
		{
			return IS.Instance.Hospital.Rooms.Where(r => !r.Deleted).ToList();
		}

		public static List<Room.RoomType> GetModifiableRoomTypes()
		{
			return Enum.GetValues(typeof(Room.RoomType)).Cast<Room.RoomType>()
				.Where(e => e != Room.RoomType.WAREHOUSE)
				.ToList();
		}

		public static bool HasEquipmentRightNow(Room room, Equipment equipment)
		{
			return room.Equipment.ContainsKey(equipment) && room.Equipment[equipment] > 0;
		}

		public static bool HasEquipmentRightAfterRelocations(Room room, Equipment equipment)
		{
			var relevantRelocations = EquipmentRelocationController.GetRelocationsFrom(room, equipment);
			return room.Equipment.ContainsKey(equipment) && room.Equipment[equipment] - relevantRelocations.Count > 0;
		}

		public static List<Equipment> GetEquipment(Room room)
		{
			return EquipmentController.GetEquipment().Where(eq => HasEquipmentRightAfterRelocations(room, eq)).ToList();
		}
	}
}
