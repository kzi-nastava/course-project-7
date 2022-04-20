using Newtonsoft.Json;
using System;

namespace HospitalIS.Backend
{
	internal class EquipmentRelocation : Entity
	{
		public Equipment Equipment { get; set; }
		public Room RoomNew { get; set; }
		public DateTime ScheduledFor { get; set; }

		public EquipmentRelocation(Equipment equipment, Room roomNew, DateTime scheduledFor)
		{
			Equipment = equipment;
			RoomNew = roomNew;
			ScheduledFor = scheduledFor;
		}

		public EquipmentRelocation()
		{
		}

		public override string ToString()
		{
			return $"EquipmentRelocation{{Id = {Id}, Equipment = {Equipment.Id}, RoomNew = {RoomNew.Id} ({RoomNew.Name}), ScheduledFor = {ScheduledFor}}}";
		}
	}
}
