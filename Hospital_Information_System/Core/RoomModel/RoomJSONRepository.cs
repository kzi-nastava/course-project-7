using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.RoomModel
{
	public class RoomJSONRepository : IRoomRepository
	{
		private readonly IList<Room> _rooms;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public RoomJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			RoomJSONReferenceConverter.Repo = this;
			_rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _rooms.Count;
		}

		public IEnumerable<Room> Get()
		{
			return _rooms.Where(o => !o.Deleted);
		}

		public Room Get(int id)
		{
			return _rooms.FirstOrDefault(r => r.Id == id);
		}

		public void Add(Room obj)
		{
			_rooms.Add(obj);
		}

		public void Remove(Room obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_rooms, Formatting.Indented, _settings));
		}
	}
}