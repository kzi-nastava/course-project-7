using System.Collections.Generic;
using Newtonsoft.Json;

namespace HospitalIS.Backend
{
	public class Room : Entity
	{
		public enum RoomType
		{
			WAREHOUSE, OPERATION, RECOVERY, EXAMINATION, BATHROOM, OTHER
		}
		public RoomType Type { get; set; }
		public string Name { get; set; }
		public int Floor { get; set; }
		[JsonIgnore]
		public List<Equipment> Equipment = new List<Equipment>();

		public Room()
		{
			Floor = 0;
			Type = RoomType.OTHER;
			Name = "";
		}

		public Room(int floor, RoomType type, string name)
		{
			Floor = floor;
			Type = type;
			Name = name;
		}
		public Room(int floor, RoomType type, int roomOrdinal = 0)
		{
			Floor = floor;
			Type = type;
			Name = $"{type.ToString().ToLower()} {floor}-{(int)type}-{roomOrdinal}";
		}
		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
