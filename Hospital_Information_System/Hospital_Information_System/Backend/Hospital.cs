using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend
{
	internal class Hospital : Entity
	{
		private static readonly JsonSerializerSettings settings;
		private static readonly string fnameRooms = "rooms.json";

		private List<Room> _rooms = new List<Room>();
		public IReadOnlyList<Room> Rooms => _rooms;
		static Hospital()
		{
			settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
		}
		public void Save(string directory)
		{
			File.WriteAllText(Path.Combine(directory, fnameRooms), JsonConvert.SerializeObject(Rooms, Formatting.Indented, settings));
		}
		public void Load(string directory)
		{
			_rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(Path.Combine(directory, fnameRooms)), settings);
		}
		public void Add(Room room)
		{
			room.Id = (Rooms.Last()?.Id ?? -1) + 1;
			_rooms.Add(room);
		}
	}
}
