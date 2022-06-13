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

		public IEnumerable<Room> GetAll()
		{
			return _rooms.Where(o => !o.Deleted);
		}

		public Room Get(int id)
		{
			return _rooms.FirstOrDefault(r => r.Id == id);
		}

		public Room Add(Room obj)
		{
			obj.Id = GetNextId();
			_rooms.Add(obj);
			return obj;
		}

		public void Remove(Room obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_rooms, Formatting.Indented, _settings));
		}

		public IEnumerable<Room> GetModifiable()
		{
			return GetAll().Where(r => r.Type != RoomType.WAREHOUSE);
		}

		public IEnumerable<Room> GetExaminationRooms()
		{
			return GetModifiable().Where(r => r.Type == RoomType.EXAMINATION);
		}

		public IEnumerable<Room> GetOtherModifiableOnSameFloor(Room comparedTo)
		{
			return GetModifiable().Where(r => r.Floor == comparedTo.Floor && r.Id != comparedTo.Id);
		}
    }
}