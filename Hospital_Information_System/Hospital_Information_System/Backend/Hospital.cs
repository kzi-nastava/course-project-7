using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend
{
	class Hospital : Entity
	{
		private static string fnameRooms = "rooms.json";
		private static JsonSerializerSettings settings = new JsonSerializerSettings{ PreserveReferencesHandling = PreserveReferencesHandling.Objects};
		internal List<Room> Rooms { get; set; } = new List<Room>();

		public void Save(string directory)
		{

			File.WriteAllText(Path.Combine(directory, fnameRooms), JsonConvert.SerializeObject(Rooms, Formatting.Indented, settings));
		}
		public void Load(string directory)
		{
			Rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(Path.Combine(directory, fnameRooms)), settings);
		}
		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
		public void Add(Room room)
		{
			room.Id = Rooms.LastOrDefault().Id + 1;
			Rooms.Add(room);
		}
	}
}
