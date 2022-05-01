using Newtonsoft.Json;
using System;

namespace HospitalIS.Backend
{
	internal class Renovation : Entity
	{
		[JsonConverter(typeof(Repository.RoomRepository.RoomReferenceConverter))]
		public Room Room;
		public DateTime Start;
		public DateTime End;

		public Renovation(Room room, DateTime start, DateTime end)
		{
			Room = room;
			Start = start;
			End = end;
		}

		public Renovation()
		{
		}

		public override string ToString()
		{
			return $"Renovation(Room={Room.Id} {Room.Name}, Start={Start}, End={End})";
		}
	}
}
