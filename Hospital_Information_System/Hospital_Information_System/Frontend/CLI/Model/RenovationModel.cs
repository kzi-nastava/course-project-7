using System;
using System.Collections.Generic;
using System.Linq;

using HospitalIS.Backend;
using HospitalIS.Backend.Controller;

using System.Diagnostics;

namespace HospitalIS.Frontend.CLI.Model
{
    internal static class RenovationModel
    {
        struct Interval
        {
            public DateTime Start;
            public DateTime End;

            public Interval(DateTime start, DateTime end)
            {
                Debug.Assert(start < end);
                Start = start;
                End = end;
            }

            public bool Contains(DateTime dt)
            {
                return Start <= dt && dt <= End;
            }

            public override string ToString()
            {
                const string format = "dd.MM.yyyy. HH:mm:ss";
                return $"{Start.ToString(format)} - {End.ToString(format)}";
            }
        }

        private const string hintSelectRoom = "Select room for renovation";
        private const string hintSelectStart = "Select renovation starting time";
        private const string hintSelectEnd = "Select renovation ending time";
        private const string infoUnavailable = "Renovation cannot be scheduled during the following times";
        private const string errDateCutsBusyRoomTime = "The room is in use during that time";
		private const string errEndDateNotBeforeStart = "Renovation cannot end before its starting timestamp";

        public static void NewRenovation(string inputCancelString)
        {
            var renovation = InputRenovation(inputCancelString);
            IS.Instance.RenovationRepo.Add(renovation);
        }

        private static Renovation InputRenovation(string inputCancelString)
        {
            Renovation renovation = new Renovation();
			Console.WriteLine(hintSelectRoom);
            renovation.Room = InputRoom(inputCancelString);
            PrintUnavailableTimeslotsForRenovation(renovation.Room);
			Console.WriteLine(hintSelectStart);
            renovation.Start = InputStart(inputCancelString, renovation);
			Console.WriteLine(hintSelectEnd);
            renovation.End = InputEnd(inputCancelString, renovation);
            return renovation;
        }

        private static Room InputRoom(string inputCancelString)
        {
            return EasyInput<Room>.Select(
                RoomController.GetModifiableRooms(),
                inputCancelString
            );
        }

        private static DateTime InputStart(string inputCancelString, Renovation reference)
        {
            Debug.Assert(reference.Room != null);
            var badDates = GetUnavailableTimeslotsForRenovation(reference.Room);

            return EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>> { dt => badDates.Count(bd => bd.Contains(dt)) == 0 },
                new[] { errDateCutsBusyRoomTime },
                inputCancelString
            );
        }

        private static DateTime InputEnd(string inputCancelString, Renovation reference)
        {
            Debug.Assert(reference.Room != null);
            var badDates = GetUnavailableTimeslotsForRenovation(reference.Room);

            return EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>>
                {
                    dt => badDates.Count(bd => bd.Contains(dt)) == 0,
                    dt => reference.Start <= dt
                },
                new[]
                {
                    errDateCutsBusyRoomTime,
					errEndDateNotBeforeStart
                },
                inputCancelString
            );
        }

        private static void PrintUnavailableTimeslotsForRenovation(Room r)
        {

            var unavailableSlotsSorted = GetUnavailableTimeslotsForRenovation(r);
            unavailableSlotsSorted.Sort((a, b) => (a.Start.CompareTo(b.Start)));

            Console.WriteLine(infoUnavailable);
            foreach (var interval in unavailableSlotsSorted)
            {
                Console.WriteLine(interval);
            }
        }

        private static List<Interval> GetUnavailableTimeslotsForRenovation(Room r)
        {
            List<Interval> result = new List<Interval>();

            var relevantRenovations = RenovationController.GetRenovations().Where(ren => ren.Room == r).ToList();
            foreach (var ren in relevantRenovations)
            {
                result.Add(new Interval(ren.Start, ren.End));
            }

            var relevantAppointments = AppointmentController.GetAppointments().Where(ap => ap.Room == r).ToList();
            foreach (var ap in relevantAppointments)
            {
                // TODO @magley: Utilize "duration" property once it gets implemented into Appointments.
                DateTime start = ap.ScheduledFor;
                DateTime end = start.AddMinutes(AppointmentController.LengthOfAppointmentInMinutes);
                result.Add(new Interval(start, end));
            }

            return result;
        }
    }
}
