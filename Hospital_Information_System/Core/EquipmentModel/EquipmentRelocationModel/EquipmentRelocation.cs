using HIS.Core.Foundation;
using HIS.Core.RoomModel;
using System;
using Newtonsoft.Json;

namespace HIS.Core.EquipmentModel.EquipmentRelocationModel
{
	public enum EquipmentRelocationProperty { EQUIPMENT, ROOMFROM, ROOMTO, WHEN };

	public class EquipmentRelocation : Entity
	{
		[JsonConverter(typeof(EquipmentJSONReferenceConverter))]
		public Equipment Equipment { get; set; }

		[JsonConverter(typeof(RoomJSONReferenceConverter))]
		public Room RoomTo { get; set; }

		[JsonConverter(typeof(RoomJSONReferenceConverter))]
		public Room RoomFrom { get; set; }

		public DateTime When { get; set; } = DateTime.MinValue;

		public EquipmentRelocation() {}

		public EquipmentRelocation(Equipment equipment, Room from, Room to, DateTime scheduledFor)
		{
			Equipment = equipment;
			RoomFrom = from;
			RoomTo = to;
			When = scheduledFor;
		}

		public override string ToString()
		{
			return $"EquipmentRelocation{{Id = {Id}, Equipment = {Equipment.Id}, RoomFrom = {RoomFrom.Id} ({RoomFrom.Name}), RoomTo = {RoomTo.Id} ({RoomTo.Name}), When = {When}}}";
		}
	}
}
