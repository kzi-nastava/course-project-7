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
			if (properties.Contains(Property.NEW_ROOM)) dest.RoomNew = src.RoomNew;
			if (properties.Contains(Property.WHEN_TO_RELOCATE)) dest.ScheduledFor = src.ScheduledFor;
		}

		public static List<EquipmentRelocation> GetRelocations()
		{
			return IS.Instance.Hospital.EquipmentRelocations.Where(r => !r.Deleted).ToList();
		}

		public static bool CanBeOldRoomFor(Room room, EquipmentRelocation reference)
		{ 
			return reference.RoomOld == null // RoomOld isn't input yet which means that nothing is input yet
			|| reference.Equipment == null // Equipment isn't input yet so it's fair game
			|| RoomController.HasEquipment(room, reference.Equipment); // 'room' has enough equipment to remove

			// TODO @magley: The last check isn't enough. What if there are pending relocations for 'room' which,
			// together with the new relocation, exceed the equipment count in that room? This also applies to
			// regular relocation create operation.
		}
	}
}
