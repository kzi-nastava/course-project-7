using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend.Room
{
	public class RoomJSONRepository : IRoomRepository
	{
		private List<Room> _rooms;
		private string _fname;
		private JsonSerializerSettings _settings;

		public RoomJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			_rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(fname), _settings);
		}

		public IEnumerable<Room> Get()
		{
			return _rooms;
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
			_rooms.Remove(obj);
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_rooms, Formatting.Indented, _settings));
		}
	}

	internal class EquipmentDictionaryConverter<T> : JsonConverter
	{
		internal static IEquipmentRepository repo;

		public EquipmentDictionaryConverter()
		{
			if (repo == null) throw new JSONRepoReferenceNullException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var writableDict = new Dictionary<int, T>();
			foreach (var kv in (Dictionary<Equipment, T>)value)
			{
				writableDict[kv.Key.Id] = kv.Value;
			}

			serializer.Serialize(writer, writableDict);
		}

		public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
		{
			var readableDict = serializer.Deserialize<Dictionary<int, T>>(reader);
			var result = new Dictionary<Equipment, T>();
			foreach (var kv in readableDict)
			{
				Equipment eq = repo.Get().First(eq => eq.Id == kv.Key);
				result[eq] = kv.Value;
			}
			return result;
		}

		public override bool CanConvert(System.Type objectType)
		{
			return objectType == typeof(Dictionary<Equipment, T>);
		}
	}
}