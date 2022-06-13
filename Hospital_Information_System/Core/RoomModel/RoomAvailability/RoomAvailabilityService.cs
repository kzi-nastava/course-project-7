using HIS.Core.AppointmentModel;
using HIS.Core.AppointmentModel.Util;
using HIS.Core.RoomModel.RenovationModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.RoomModel.RoomAvailability
{
	public class RoomAvailabilityService : IRoomAvailabilityService
	{
		private IRoomService _roomService;
		private IRenovationService _renovationService;
		private IAppointmentService _appointmentService;

		public RoomAvailabilityService()
		{
		}

		public RoomAvailabilityService(IRoomService roomService, IRenovationService renovationService, IAppointmentService appointmentService)
		{
			_roomService = roomService;
			_renovationService = renovationService;
			_appointmentService = appointmentService;
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

		public bool IsAvailable(Room room, DateTime newSchedule, Appointment refAppointment = null)
		{
			var relevantAppointments = _appointmentService.GetAll().Where(ap => ap != refAppointment && ap.Room == room);
			foreach (var Appointment in relevantAppointments)
			{
				if (_appointmentService.AreColliding(Appointment.ScheduledFor, newSchedule))
				{
					return false;
				}

				// TODO @magley: This will have to change once appointments have a variable duration.

				if (_renovationService.IsRenovating(room, newSchedule, newSchedule.AddMinutes(AppointmentConstants.LengthOfAppointmentInMinutes)))
				{
					return false;
				}
			}

			return true;
		}

		public Room FindFirstAvailableExaminationRoom(DateTime scheduledFor)
		{
			return _roomService.GetExaminationRooms().First(r => IsAvailable(r, scheduledFor));
		}

        public IEnumerable<Room> GetAvailableExaminationRooms(Appointment refAppointment)
		{
			if (refAppointment == null)
			{
				return _roomService.GetExaminationRooms();
			}

			return _roomService.GetExaminationRooms().Where(d => IsAvailable(d, refAppointment.ScheduledFor, refAppointment));
		}

        public Room GetRandomAvailableExaminationRoom(Appointment refAppointment)
		{
			var rnd = new Random();
			List<Room> rooms = (List<Room>)GetAvailableExaminationRooms(refAppointment);
			return rooms[rnd.Next(rooms.Count)];
		}
    }
}
