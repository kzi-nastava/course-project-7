using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Controller
{
	internal class EquipmentRelocationController
	{

		public enum Property
		{
			EQUIPMENT,
			OLD_ROOM,
			NEW_ROOM,
			WHEN_TO_RELOCATE
		}

		public static List<Property> GetRelocationProperties()
		{
			return Enum.GetValues(typeof(Property)).Cast<Property>().ToList();
		}

		public static void Copy(EquipmentRelocation dest, EquipmentRelocation src, List<Property> properties)
		{
			if (properties.Contains(Property.EQUIPMENT)) dest.Equipment = src.Equipment;
			if (properties.Contains(Property.OLD_ROOM)) dest.RoomOld = src.RoomOld;
			if (properties.Contains(Property.NEW_ROOM)) dest.RoomNew = src.RoomNew;
			if (properties.Contains(Property.WHEN_TO_RELOCATE)) dest.ScheduledFor = src.ScheduledFor;
		}

		public static List<EquipmentRelocation> GetRelocations()
		{
			return IS.Instance.Hospital.EquipmentRelocations.Where(r => !r.Deleted).ToList();
		}

		public static List<EquipmentRelocation> GetRelocationsFrom(Room room, Equipment equipment)
		{
			return IS.Instance.Hospital.EquipmentRelocations.Where(r => r.RoomOld == room && r.Equipment == equipment).ToList();
		}

		public static List<EquipmentRelocation> GetRelocationsTo(Room room, Equipment equipment)
		{
			return IS.Instance.Hospital.EquipmentRelocations.Where(r => r.RoomNew == room && r.Equipment == equipment).ToList();
		}

		public static bool CanBeOldRoom(Room room, EquipmentRelocation reference, bool dynamicRoom = false)
		{
			if (dynamicRoom && reference.RoomNew == room)
				return false;
			
			if (dynamicRoom && reference.RoomOld == null)
				return room.Equipment.Count > 0 && IsRoomWithDynamicEquipment(room);
			
			if (reference.RoomOld == null && !dynamicRoom)
			{
				return room.Equipment.Count > 0;
			}

			if (reference.Equipment == null)
			{
				return true;
			}

			return RoomController.HasEquipmentAfterRelocations(room, reference.Equipment);
		}
		private static bool IsRoomWithDynamicEquipment(Room room)
		{
			return (room.Type == Room.RoomType.OPERATION || room.Type == Room.RoomType.EXAMINATION ||
			        room.Type == Room.RoomType.WAREHOUSE);
		}
		// public static bool CanBeOldRoomWithDynamicEquipment(Room room, EquipmentRelocation reference)
		// {
		// 	if (reference.RoomNew == room)
		// 		return false;
		// 	
		// 	if (reference.RoomOld == null)
		// 	{
		// 		return room.Equipment.Count > 0 && IsRoomWithDynamicEquipment(room);
		// 	}
		//
		// 	if (reference.Equipment == null)
		// 	{
		// 		return true;
		// 	}
		//
		// 	return RoomController.HasEquipmentAfterRelocations(room, reference.Equipment);
		// }
	}
}
