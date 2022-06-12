using Newtonsoft.Json;
using System;

namespace HospitalIS.Backend
{
	/*
	internal class EquipmentRelocation2 : Entity
	{
		[JsonConverter(typeof(Repository.EquipmentRepository.EquipmentReferenceConverter))]
		public Equipment Equipment { get; set; } = null;

		[JsonConverter(typeof(Repository.RoomRepository.RoomReferenceConverter))]
		public Room RoomNew { get; set; } = null;

		[JsonConverter(typeof(Repository.RoomRepository.RoomReferenceConverter))]
		public Room RoomOld { get; set; } = null;

		public DateTime ScheduledFor { get; set; } = DateTime.MinValue;

		public EquipmentRelocation2(Equipment equipment, Room roomOld, Room roomNew, DateTime scheduledFor)
		{
			Equipment = equipment;
			RoomOld = roomOld;
			RoomNew = roomNew;
			ScheduledFor = scheduledFor;
		}

		public EquipmentRelocation2()
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
	*/
}
