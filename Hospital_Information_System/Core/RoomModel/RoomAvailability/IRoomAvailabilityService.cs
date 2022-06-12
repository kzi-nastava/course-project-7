using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.RoomModel.RoomAvailability
{
	public interface IRoomAvailabilityService
	{
		public bool IsAvailable(Room r, DateTimeRange dateTimeRange);
		public bool IsAvailable(Room r, DateTime dateTime);
		public IList<DateTimeRange> GetUnavailableTimes(Room r);
	}
}
