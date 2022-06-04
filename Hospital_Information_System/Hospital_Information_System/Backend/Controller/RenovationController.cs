using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Util;
using System.Threading;
using System.Threading.Tasks;

namespace HospitalIS.Backend.Controller
{
	internal static class RenovationController
	{
		public static List<Renovation> GetRenovations()
		{
			return IS.Instance.Hospital.Renovations.Where(ren => !ren.Deleted).ToList();
		}

		public static bool IsRenovating(Room room, DateTime timestamp)
		{
			return GetRenovations().Count(ren => ren.Room == room && ren.Start <= timestamp && timestamp <= ren.End) > 0;
		}

		public static bool IsRenovating(Room room, DateTime start, DateTime end)
		{
			return GetRenovations().Count(ren => 
				ren.Room == room && 
				new DateTimeRange(start, end).Intersects(new DateTimeRange(ren.Start, ren.End))) > 0;
		}

		public static void Schedule(Renovation renovation)
		{
			IS.Instance.RenovationRepo.Add(renovation);
			AddTask(renovation);
		}

		public static List<DateTimeRange> GetUnavailableTimeslotsFromRenovations(Room r)
		{
			List<DateTimeRange> result = new List<DateTimeRange>();

			var relevantRenovations = RenovationController.GetRenovations().Where(ren => ren.Room == r).ToList();
			foreach (var ren in relevantRenovations)
			{
				if (ren.IsSplitting() || ren.IsMerging())
				{
					result.Add(new DateTimeRange(ren.Start, DateTime.MaxValue));
				}
				else
				{
					result.Add(new DateTimeRange(ren.Start, ren.End));
				}
			}

			return result;
		}

		public static List<DateTimeRange> GetUnavailableTimeslotsFromAppointments(Room r)
		{
			List<DateTimeRange> result = new List<DateTimeRange>();

			var relevantAppointments = AppointmentController.GetAppointments().Where(ap => ap.Room == r).ToList();
			foreach (var ap in relevantAppointments)
			{
				// TODO @magley: Utilize "duration" property once it gets implemented into Appointments.
				DateTime start = ap.ScheduledFor;
				DateTime end = start.AddMinutes(AppointmentController.LengthOfAppointmentInMinutes);
				result.Add(new DateTimeRange(start, end));
			}

			return result;
		}

		public static List<DateTimeRange> GetUnavailableTimeslotsForRenovation(Room r)
		{
			var result = GetUnavailableTimeslotsFromRenovations(r);
			result.AddRange(GetUnavailableTimeslotsFromAppointments(r));
			return result;
		}

		public static List<Renovation> GetInvalidRenovationsAfterScheduling(Renovation renovation)
		{
			if (renovation.IsSplitting() || renovation.IsMerging())
			{
				return RenovationController.GetRenovations().Where(ren => ren.Room == renovation.Room && ren.Start >= renovation.End).ToList();
			}
			return new List<Renovation>();
		}

		private static void ExecuteSplit(Renovation renovation)
		{
			IS.Instance.RoomRepo.Add(renovation.SplitRoomTarget1);
			IS.Instance.RoomRepo.Add(renovation.SplitRoomTarget2);
			IS.Instance.RoomRepo.Remove(renovation.Room);
		}

		private static void ExecuteMerge(Renovation renovation)
		{
			if (IS.Instance.RoomRepo.GetById(renovation.MergeRoomTarget.Id) == null)
			{
				IS.Instance.RoomRepo.Add(renovation.MergeRoomTarget);
			}

			foreach (var kv in renovation.Room.Equipment)
			{
				renovation.MergeRoomTarget.Equipment.Add(kv.Key, kv.Value);
			}

			IS.Instance.RoomRepo.Remove(renovation.Room);
		}

		public static void Execute(Renovation renovation)
		{
			Thread.Sleep(Math.Max(renovation.GetTimeToLive(), 0));

			if (renovation.Deleted)
				return;

			if (!IS.Instance.Hospital.Renovations.Contains(renovation))
				throw new EntityNotFoundException();

			Console.WriteLine($"Finished renovation {renovation}.");

			if (renovation.IsSplitting())
				ExecuteSplit(renovation);
			else if (renovation.IsMerging())
				ExecuteMerge(renovation);

			IS.Instance.RenovationRepo.Remove(renovation);
		}

		public static void AddTask(Renovation renovation)
		{
			Task t = new Task(() => Execute(renovation));
			IS.Instance.Hospital.RenovationTasks.Add(t);
			t.Start();
		}
	}
}
