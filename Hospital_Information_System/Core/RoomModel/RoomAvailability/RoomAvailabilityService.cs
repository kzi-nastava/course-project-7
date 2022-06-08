using HIS.Core.RoomModel.RenovationModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.RoomModel.RoomAvailability
{
	public class RoomAvailabilityService : IRoomAvailabilityService
	{
		private IRoomService _roomService;
		private IRenovationService _renovationService;

		public RoomAvailabilityService()
		{
		}

		public RoomAvailabilityService(IRoomService roomService, IRenovationService renovationService)
		{
			_roomService = roomService;
			_renovationService = renovationService;
		}

		public IList<DateTimeRange> GetUnavailableTimes(Room r)
		{
			IList<DateTimeRange> unavailableTimes = new List<DateTimeRange>();

			var relevantRenovations = _renovationService.GetAll(r);

			foreach (var renovation in relevantRenovations)
			{
				unavailableTimes.Add(renovation.TimeRange);
			}

			return unavailableTimes;
		}

		public bool IsAvailable(Room r, DateTimeRange dateTimeRange)
		{
			throw new NotImplementedException();
		}

		public bool IsAvailable(Room r, DateTime dateTime)
		{
			throw new NotImplementedException();
		}
	}
}
