using Newtonsoft.Json;
using System;

namespace HospitalIS.Backend
{
	internal class EquipmentRelocation : Entity
	{
		[JsonConverter(typeof(Repository.EquipmentRepository.EquipmentReferenceConverter))]
		public Equipment Equipment { get; set; } = null;

		[JsonConverter(typeof(Repository.RoomRepository.RoomReferenceConverter))]
		public Room RoomNew { get; set; } = null;

		[JsonConverter(typeof(Repository.RoomRepository.RoomReferenceConverter))]
		public Room RoomOld { get; set; } = null;

		public DateTime ScheduledFor { get; set; } = null;

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

		public int GetTimeToLive()
		{
			return (int)(ScheduledFor - DateTime.Now).TotalMilliseconds;
		}
	}
}
