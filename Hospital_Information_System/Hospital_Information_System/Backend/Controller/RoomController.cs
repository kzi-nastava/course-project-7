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
			return IS.Instance.Hospital.Rooms.Where(r => !r.Deleted && r.Type != Room.RoomType.WAREHOUSE).ToList();
		}

		public static List<Room.RoomType> GetModifiableRoomTypes()
		{
			return Enum.GetValues(typeof(Room.RoomType)).Cast<Room.RoomType>()
				.Where(e => e != Room.RoomType.WAREHOUSE)
				.ToList();
		}
	}
}
