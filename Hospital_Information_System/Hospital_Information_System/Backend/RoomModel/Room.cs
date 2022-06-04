using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.EquipmentModel;
using HospitalIS.Backend.Foundation;

namespace HospitalIS.Backend.RoomModel
{
	public enum RoomType
	{
		WAREHOUSE, OPERATION, RECOVERY, EXAMINATION, BATHROOM, OTHER
	}

	public enum RoomProperty
	{
		NAME, FLOOR, TYPE, EQUIPMENT
	}

	public class Room : Entity
	{
		public string Name { get; set; }
		public int Floor { get; set; }
		public RoomType Type { get; set; }

		[JsonConverter(typeof(EquipmentJSONDictionaryConverter<int>))]
		public Dictionary<Equipment, int> Equipment { get; set; }

		#region Constructors, ToString
		public Room()
		{
		}

		public Room(int floor, RoomType type, string name)
		{
			Floor = floor;
			Type = type;
			Name = name;
			Equipment = new Dictionary<Equipment, int>();
		}

		public override string ToString()
		{
			return $"Room{{Id = {Id}, Name = {Name}, Floor = {Floor}, Type = {Type}}}";
		}
		#endregion
	}
}
