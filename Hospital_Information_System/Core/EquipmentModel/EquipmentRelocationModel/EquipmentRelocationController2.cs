using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Controller
{
	/*
	internal class EquipmentRelocationController2
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

		public static void Copy(EquipmentRelocation2 dest, EquipmentRelocation2 src, List<Property> properties)
		{
			if (properties.Contains(Property.EQUIPMENT)) dest.Equipment = src.Equipment;
			if (properties.Contains(Property.OLD_ROOM)) dest.RoomOld = src.RoomOld;
			if (properties.Contains(Property.NEW_ROOM)) dest.RoomNew = src.RoomNew;
			if (properties.Contains(Property.WHEN_TO_RELOCATE)) dest.ScheduledFor = src.ScheduledFor;
		}

		public static List<EquipmentRelocation2> GetRelocations()
		{
			return IS.Instance.Hospital.EquipmentRelocations.Where(r => !r.Deleted).ToList();
		}

		public static List<EquipmentRelocation2> GetRelocationsFrom(Room room, Equipment equipment)
		{
			return IS.Instance.Hospital.EquipmentRelocations.Where(r => r.RoomOld == room && r.Equipment == equipment).ToList();
		}

		public static List<EquipmentRelocation2> GetRelocationsTo(Room room, Equipment equipment)
		{
			return IS.Instance.Hospital.EquipmentRelocations.Where(r => r.RoomNew == room && r.Equipment == equipment).ToList();
		}

		public static bool CanBeOldRoom(Room room, EquipmentRelocation2 reference)
		{
			if (reference.RoomOld == null)
			{
				return room.Equipment.Count > 0;
			}

			if (reference.Equipment == null)
			{
				return true;
			}

			return RoomController.HasEquipmentAfterRelocations(room, reference.Equipment);
		}
	}
	*/
}
