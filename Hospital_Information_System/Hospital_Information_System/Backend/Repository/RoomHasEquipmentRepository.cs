using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace HospitalIS.Backend.Repository
{
	class RoomHasEquipmentJSON
	{
		public int RoomID = -1;
		public List<int> EquipmentIDs = new List<int>();
		internal RoomHasEquipmentJSON()
		{
		}
		internal RoomHasEquipmentJSON(Room room)
		{
			RoomID = room.Id;
			EquipmentIDs = (from eq in room.Equipment select eq.Id).ToList();
		}

		internal List<Equipment> GetEquipmentOf(Hospital hospital)
		{
			return hospital.Equipment.Where(eq => EquipmentIDs.Contains(eq.Id)).ToList();
		}
	}
	internal class RoomHasEquipmentRepository
	{
		internal static void Save(Hospital hospital, string fullFilename, JsonSerializerSettings settings)
		{
			var data = 
				(from room in hospital.Rooms
				select new RoomHasEquipmentJSON(room)).ToList();

			File.WriteAllText(fullFilename, JsonConvert.SerializeObject(data, Formatting.Indented, settings));
		}

		internal static void Load(Hospital hospital, string fullFilename, JsonSerializerSettings settings)
		{
			string text = File.ReadAllText(fullFilename);
			var data = JsonConvert.DeserializeObject<List<RoomHasEquipmentJSON>>(text, settings);
			data.ForEach(datum => hospital.Rooms[datum.RoomID].Equipment = datum.GetEquipmentOf(hospital));
		}
	}
}
