using Newtonsoft.Json;
using System;

namespace HospitalIS.Backend
{
	class RenovationAccessorException : Exception {
		public RenovationAccessorException(string msg = ""): base(msg) {}
	}

	internal class Renovation : Entity
	{
		[JsonConverter(typeof(Repository.RoomJSONReferenceConverter))]
		public Room Room;
		public DateTime Start;
		public DateTime End;

		// These are stored directly because they don't exist in the system unti the renovation ends
		
		[JsonProperty]
		private Room _room1 = null;

		[JsonProperty]
		private Room _room2 = null;

		/// <summary>
		/// The first of the two rooms that Room is split in.
		/// </summary>
		[JsonIgnore]
		public Room SplitRoomTarget1 {
			get {
				return _room1;
			}
			set {
				_room1 = value;
			}
		}

		/// <summary>
		/// The second of the two rooms that Room is split in.
		/// </summary>
		[JsonIgnore]
		public Room SplitRoomTarget2 {
			get {
				return _room2;
			}
			set {
				_room2 = value;
			}
		}

		/// <summary>
		/// The room that Room will be merged into.
		/// </summary>
		[JsonIgnore]
		public Room MergeRoomTarget {
			get {
				return _room1;
			}
			set {
				_room1 = value;
			}
		}

		public Renovation(Room room, DateTime start, DateTime end, Room room1 = null, Room room2 = null)
		{
			Room = room;
			Start = start;
			End = end;
			_room1 = room1;
			_room2 = room2;
		}

		public bool IsSplitting() {
			return _room1 != null && _room2 != null;
		}

		public bool IsMerging() {
			return _room1 != null && _room2 == null;
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
