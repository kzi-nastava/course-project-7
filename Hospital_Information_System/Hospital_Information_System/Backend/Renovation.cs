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
		
		/// <summary>
		/// If the renovation splits this room into two rooms, then this will be the first of the 2.
		/// If the renovation merges this room into a new room, then this will be the new room.
		/// Otherwise, this is null.
		/// </summary>
		public Room Room1 = null;

		/// <summary>
		/// If the renovation splits this room into two rooms, then this will be the second of the 2.
		/// If the renovation merges this room into a new room, then this will be null.
		/// Otherwise, this is null.
		/// </summary>
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

		public bool IsMerging() {
			return Room1 != null && Room2 == null;
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
