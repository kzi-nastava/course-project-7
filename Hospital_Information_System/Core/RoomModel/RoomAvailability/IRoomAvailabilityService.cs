using HIS.Core.AppointmentModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.RoomModel.RoomAvailability
{
	public interface IRoomAvailabilityService
	{
		public bool IsAvailable(Room r, DateTimeRange dateTimeRange);
		public bool IsAvailable(Room room, DateTime newSchedule, Appointment refAppointment = null);
		public IList<DateTimeRange> GetUnavailableTimes(Room r);
		public IEnumerable<Room> GetAvailableExaminationRooms(Appointment refAppointment);
		public Room GetRandomAvailableExaminationRoom(Appointment refAppointment);
		public Room FindFirstAvailableExaminationRoom(DateTime scheduledFor);
	}
}
