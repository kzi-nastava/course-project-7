using Newtonsoft.Json;
using System;

namespace HospitalIS.Backend
{
	internal class EquipmentRelocation : Entity
	{
		public Equipment Equipment { get; set; }
		public Room RoomOld { get; set; }
		public Room RoomNew { get; set; }
		public DateTime ScheduledFor { get; set; }

		public EquipmentRelocation(Equipment equipment, Room roomOld, Room roomNew, DateTime scheduledFor)
		{
			Equipment = equipment;
			RoomOld = roomOld;
			RoomNew = roomNew;
			ScheduledFor = scheduledFor;
		}

		public EquipmentRelocation()
		{
		}

		public override string ToString()
		{
			return $"EquipmentRelocation{{Id = {Id}, Equipment = {Equipment.Id}, RoomOld = {RoomOld.Id} ({RoomOld.Name}), RoomNew = {RoomNew.Id} ({RoomNew.Name}), ScheduledFor = {ScheduledFor}}}";
		}
	}
}
