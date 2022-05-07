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

		// These are stored directly because they don't exist in the system unti the renovation ends
		public Room Room1 = null;
		public Room Room2 = null;

		public Renovation(Room room, DateTime start, DateTime end, Room room1 = null, Room room2 = null)
		{
			Room = room;
			Start = start;
			End = end;
			Room1 = room1;
			Room2 = room2;
		}

		public bool IsSplitting() {
			return Room1 != null && Room2 != null;
		}

		public Renovation()
		{
		}

		public override string ToString()
		{
			return $"Renovation(Room={Room.Id} {Room.Name}, Start={Start}, End={End})";
		}

		public int GetTimeToLive()
		{
			return (int)(End - DateTime.Now).TotalMilliseconds;
		}
	}
}
