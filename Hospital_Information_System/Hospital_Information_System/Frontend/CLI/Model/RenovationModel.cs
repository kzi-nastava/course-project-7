using System;
using System.Collections.Generic;
using System.Linq;

using HospitalIS.Backend;
using HospitalIS.Backend.Controller;

using System.Diagnostics;
using HospitalIS.Backend.Util;

namespace HospitalIS.Frontend.CLI.Model
{
	internal static class RenovationModel
	{
		private const string hintSelectRoom = "Select room for renovation";
		private const string hintSelectStart = "Select renovation starting time";
		private const string hintSelectEnd = "Select renovation ending time";
		private const string infoUnavailable = "Renovation cannot be scheduled during the following times";
		private const string errDateCutsBusyRoomTime = "The room is in use during that time";
		private const string errEndDateNotBeforeStart = "Renovation cannot end before its starting timestamp";
		private const string errStartDateBeforeNow = "Renovation cannot be scheduled into the past!";
		private const string askWantsToSplit = "Will this room split into 2 new rooms?";
		private const string hintSelectEquipmentForSplit = "Select equipment to be moved into the first room. The remaining equipment will be moved into the second room";
		private const string askToRemoveInvalidRenovations = "There exist renovations for this room that will no longer be valid. Would you like to remove them or cancel creating this renovation?";
		private const string askWantsToMerge = "Will this room be merged with another room?";
		private const string hintMergeSelectOtherRoom = "Select other room that will be merged with this one";
		private const string hintMergeInputNewRoom = "Input data for the room that will be created after merging";
		private const string hintNoEquipment = "No equipment found to move";

		private static void Schedule(Renovation renovation)
		{
			IS.Instance.RenovationRepo.Add(renovation);
			IS.Instance.RenovationRepo.AddTask(renovation);
		}

		public static void NewRenovation(string inputCancelString)
		{
			var renovation = InputRenovation(inputCancelString);
			Schedule(renovation);
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

			bool didSplit = InputRenovationTrySplit(renovation, inputCancelString);
			if (!didSplit) {
				InputRenovationTryMerge(renovation, inputCancelString);
			}

			return renovation;
		}

		private static bool InputRenovationTrySplit(Renovation renovation, string inputCancelString) 
		{
			if (WantToSplitRoom(renovation, inputCancelString))
			{
				InputSplitRoom(renovation, inputCancelString);
				InputRemoveInvalidRenovationsAfterScheduling(renovation, inputCancelString);
				return true;
			}
			return false;
		}

		private static void InputRenovationTryMerge(Renovation renovation, string inputCancelString) 
		{
			var otherRoomRenovation = InputMergeRooms(renovation, inputCancelString);
			InputRemoveInvalidRenovationsAfterScheduling(renovation, inputCancelString);

			if (otherRoomRenovation != null)
			{
				InputRemoveInvalidRenovationsAfterScheduling(otherRoomRenovation, inputCancelString);
				Schedule(otherRoomRenovation);
			}
		}

		private static void InputRemoveInvalidRenovationsAfterScheduling(Renovation renovation, string inputCancelString)
		{
			var invalidRenovations = GetInvalidRenovationsAfterScheduling(renovation);

			if (invalidRenovations.Count() > 0)
			{
				Console.WriteLine(askToRemoveInvalidRenovations);
				if (EasyInput<bool>.YesNo(inputCancelString))
				{
					foreach (var ren in invalidRenovations)
					{
						IS.Instance.RenovationRepo.Remove(ren);
					}
				}
				else
				{
					throw new InputCancelledException();
				}
			}
		}
		private static Renovation InputMergeRooms(Renovation renovation, string inputCancelString)
		{
			Console.WriteLine(askWantsToMerge);
			if (EasyInput<bool>.YesNo(inputCancelString))
			{
				var mergeRooms = MergeRooms(renovation, inputCancelString);
				var newRoom = mergeRooms.Item1;
				var otherRoom = mergeRooms.Item2;
				renovation.MergeRoomTarget = newRoom;
				var otherRoomRenovation = new Renovation(otherRoom, renovation.Start, renovation.End, newRoom);

				return otherRoomRenovation;
			}
			return null;
		}

		private static Tuple<Room, Room> MergeRooms(Renovation renovation, string inputCancelString)
		{
			var roomPropertiesToInput = new List<RoomController.RoomProperty>{
				RoomController.RoomProperty.TYPE,
				RoomController.RoomProperty.NAME
			};

			Console.WriteLine(hintMergeInputNewRoom);
			Room newRoom = RoomModel.InputRoom(inputCancelString, roomPropertiesToInput);
			newRoom.Floor = renovation.Room.Floor;

			Console.WriteLine(hintMergeSelectOtherRoom);
			Room otherRoom = EasyInput<Room>.Select(
				RoomController.GetModifiableRooms().Where(rm => rm != renovation.Room && rm.Floor == renovation.Room.Floor).ToList(),
				inputCancelString
			);

			return new Tuple<Room, Room>(newRoom, otherRoom);
		}

		private static bool WantToSplitRoom(Renovation renovation, string inputCancelString) {
			Console.WriteLine(askWantsToSplit);
			return (EasyInput<bool>.YesNo(inputCancelString));
		}

		private static void InputSplitRoom(Renovation renovation, string inputCancelString)
		{
			var roomPropertiesToInput = new List<RoomController.RoomProperty>{
				RoomController.RoomProperty.TYPE,
				RoomController.RoomProperty.NAME
			};

			Room r1 = RoomModel.InputRoom(inputCancelString, roomPropertiesToInput);
			Room r2 = RoomModel.InputRoom(inputCancelString, roomPropertiesToInput);
			r1.Floor = renovation.Room.Floor;
			r2.Floor = renovation.Room.Floor;

			InputSplitEquipment(renovation, inputCancelString, r1, r2);

			renovation.SplitRoomTarget1 = r1;
			renovation.SplitRoomTarget2 = r2;
		}

		private static void InputSplitEquipment(Renovation renovation, string inputCancelString, Room r1, Room r2) 
		{
			try
			{
				Console.WriteLine(hintSelectEquipmentForSplit);
				var eqForRoom1 = EasyInput<KeyValuePair<Equipment, int>>.SelectMultiple(
					renovation.Room.Equipment.ToList(),
					kv => $"{kv.Key.ToString()} ({kv.Value})",
					inputCancelString
				);
				var eqForRoom2 = renovation.Room.Equipment.Except(eqForRoom1);

				Debug.Assert(eqForRoom1.Intersect(eqForRoom2).Count() == 0);
				Debug.Assert(new HashSet<KeyValuePair<Equipment, int>>(eqForRoom1.Concat(eqForRoom2)).SetEquals(renovation.Room.Equipment));

				foreach (var eqAmount in eqForRoom1) r1.Equipment.Add(eqAmount.Key, eqAmount.Value);
				foreach (var eqAmount in eqForRoom2) r2.Equipment.Add(eqAmount.Key, eqAmount.Value);
			}
			catch (NothingToSelectException)
			{
				Console.WriteLine(hintNoEquipment);
			}
		}

		private static Room InputRoom(string inputCancelString)
		{
			return EasyInput<Room>.Select(RoomController.GetModifiableRooms(), inputCancelString);
		}

		private static DateTime InputStart(string inputCancelString, Renovation reference)
		{
			Debug.Assert(reference.Room != null);
			var badDates = GetUnavailableTimeslotsForRenovation(reference.Room);

			return EasyInput<DateTime>.Get(
				new List<Func<DateTime, bool>>
				{
					dt => badDates.Count(bd => bd.Contains(dt)) == 0,
					dt => dt >= DateTime.Now
				},
				new[]
				{
					errDateCutsBusyRoomTime,
					errStartDateBeforeNow
				},
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
					dt => badDates.Count(db => db.Intersects(new DateTimeRange(reference.Start, reference.End))) == 0,
					dt => reference.Start <= dt
				},
				new[]
				{
					errDateCutsBusyRoomTime,
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

			if (unavailableSlotsSorted.Count() > 0)
			{
				Console.WriteLine(infoUnavailable);
				foreach (var interval in unavailableSlotsSorted)
				{
					Console.WriteLine(interval);
				}
			}
		}

		private static List<Renovation> GetInvalidRenovationsAfterScheduling(Renovation renovation)
		{
			if (renovation.IsSplitting() || renovation.IsMerging())
			{
				return RenovationController.GetRenovations()
					.Where(ren => ren.Room == renovation.Room && ren.Start >= renovation.End)
					.ToList();
			}
			return new List<Renovation>();
		}

		private static List<DateTimeRange> GetUnavailableTimeslotsFromRenovations(Room r) {
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

		private static List<DateTimeRange> GetUnavailableTimeslotsFromAppointments(Room r) {
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

		private static List<DateTimeRange> GetUnavailableTimeslotsForRenovation(Room r)
		{
			var result = GetUnavailableTimeslotsFromRenovations(r);
			result.AddRange(GetUnavailableTimeslotsFromAppointments(r));
			return result;
		}
	}
}
