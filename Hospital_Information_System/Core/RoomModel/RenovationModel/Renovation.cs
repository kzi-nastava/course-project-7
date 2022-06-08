using HIS.Core.EquipmentModel;
using HIS.Core.Foundation;
using HIS.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HIS.Core.RoomModel.RenovationModel
{
	public class Renovation : Entity
	{
		[JsonConverter(typeof(RoomJSONReferenceConverter))]
		public Room Room { get; set; }

		[JsonConverter(typeof(RoomJSONReferenceConverter))]
		public Room MergePairRoom { get; set; }

		public Room SplitNewRoom1 { get; set; }
		public Room SplitNewRoom2 { get; set; }

		public DateTimeRange TimeRange { get; set; }

		[JsonConverter(typeof(EquipmentJSONDictionaryConverter<int>))]
		public Dictionary<Equipment, int> EquipmentForSplitNewRoom1 { get; set; }

		public Renovation()
		{
		}

		public Renovation(Room room, Room newRoom1, Room newRoom2, DateTimeRange timeRange)
		{
			Room = room;
			SplitNewRoom1 = newRoom1;
			SplitNewRoom2 = newRoom2;
			TimeRange = timeRange;
			EquipmentForSplitNewRoom1 = new Dictionary<Equipment, int>();
		}

		public Renovation(Room room, Room mergeRoom, DateTimeRange timeRange)
		{
			Room = room;
			MergePairRoom = mergeRoom;
			TimeRange = timeRange;
			EquipmentForSplitNewRoom1 = new Dictionary<Equipment, int>();
		}

		public bool IsFinished()
		{
			return TimeRange.End >= DateTime.Now;
		}

		public bool IsSplitting()
		{
			return SplitNewRoom1 != null && SplitNewRoom2 != null;
		}

		public bool IsMerging()
		{
			return MergePairRoom != null;
		}
	}
}
