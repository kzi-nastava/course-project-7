using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.EquipmentModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.RoomModel;
using HIS.Core.RoomModel.RenovationModel;
using HIS.Core.RoomModel.RoomAvailability;
using HIS.Core.Util;

namespace HIS.CLI.View
{
	internal class RenovationView : View
	{
		private static readonly string hintInputRoom = "Select room";
		private static readonly string hintInputStart = "Select starting date and time";
		private static readonly string hintInputEnd = "Select ending date and time";
		private static readonly string hintAskSplit = "Do you wish to split the room";
		private static readonly string hintAskMerge = "Do you wish to merge the room";

		private static readonly string hintBusyRange = "Renovation schedule cannot be scheduled in the following time intervals:";
		private static readonly string errRoomBusyThen = "This room is busy at that time";
		private static readonly string errTimeInPast = "Must be in the future";
		private static readonly string errMustBeAfterStart = "Must be after the start";
		private static readonly string errMustBeNonNegative = "Must be non negative";
		private static readonly string errMustBeLessThanRoomAmount = "Must be less than what the room already has";

		private readonly IRenovationService _service;
		private readonly IRoomService _roomService;
		private readonly IRoomAvailabilityService _roomAvailabilityService;
		private readonly RoomView _roomView;

		internal RenovationView(IRenovationService service, IRoomService roomService, IRoomAvailabilityService roomAvailabilityService, RoomView roomView, UserAccount user) : base(user)
		{
			_service = service;
			_roomService = roomService;
			_roomView = roomView;
			_roomAvailabilityService = roomAvailabilityService;
		}

		internal void CmdSchedule()
		{
			Renovation renovation = InputRenovationBasic();

			Hint(hintAskSplit);
			if (EasyInput<bool>.YesNo(_cancel))
			{
				InputSplit(renovation);
			}
			else
			{
				Hint(hintAskMerge);
				if (EasyInput<bool>.YesNo(_cancel))
					InputMerge(renovation);
			}

			_service.Add(renovation);
		}

		private Renovation InputRenovationBasic()
		{
			Hint(hintInputRoom);
			var room = EasyInput<Room>.Select(_roomService.GetModifiable(), _cancel);

			var roomUnavailableTimes = _roomAvailabilityService.GetUnavailableTimes(room).ToList();
			roomUnavailableTimes.Sort((t1, t2) => t1.CompareTo(t2));
			PrintUnavailableTimes(roomUnavailableTimes);

			Hint(hintInputStart);
			var renovationStart = SelectStart(roomUnavailableTimes);
			Hint(hintInputEnd);
			var renovationEnd = SelectEnd(roomUnavailableTimes, renovationStart);

			return new Renovation(room, null, null, new DateTimeRange(renovationStart, renovationEnd));
		}

		private void PrintUnavailableTimes(IList<DateTimeRange> times)
		{
			if (times.Count() == 0)
				return;
			Hint(hintBusyRange);
			foreach (var range in times)
			{
				Hint(range.ToString());
			}
		}

		private DateTime SelectStart(IList<DateTimeRange> roomUnavailableTimes)
		{
			return EasyInput<DateTime>.Get(
				new List<Func<DateTime, bool>>()
				{
					dt => roomUnavailableTimes.Count(dtr => dtr.Contains(dt)).Equals(0),
					dt => dt > DateTime.Now
				},
				new string[]
				{
					errRoomBusyThen,
					errTimeInPast
				},
				_cancel
			);
		}

		private DateTime SelectEnd(IList<DateTimeRange> roomUnavailableTimes, DateTime start)
		{
			return EasyInput<DateTime>.Get(
				new List<Func<DateTime, bool>>()
				{
					dt => roomUnavailableTimes.Count(dtr => dtr.Intersects(new DateTimeRange(start, dt))).Equals(0),
					dt => dt > start
				},
				new string[]
				{
					errRoomBusyThen,
					errMustBeAfterStart
				},
				_cancel
			);
		}

		private void InputSplit(Renovation r)
		{
			Room room1 = _roomView.InputRoom(r.Room.Floor);
			Room room2 = _roomView.InputRoom(r.Room.Floor);
			var equipmentForRoom1 = new Dictionary<Equipment, int>();

			foreach (Equipment eq in r.Room.Equipment.Keys)
			{
				Hint($"{eq} ({r.Room.Equipment[eq]})");
				int amount = EasyInput<int>.Get(new List<Func<int, bool>>()
					{
						a => a >= 0,
						a => a <= r.Room.Equipment[eq]
					},
					new string[]
					{
						errMustBeNonNegative,
						errMustBeLessThanRoomAmount
					},
					_cancel
				);

				equipmentForRoom1[eq] = amount;
			}

			r.SplitNewRoom1 = room1;
			r.SplitNewRoom2 = room2;
			r.EquipmentForSplitNewRoom1 = equipmentForRoom1;
		}

		private void InputMerge(Renovation r)
		{
			var roomsOnSameFloor = _roomService.GetOtherModifiableOnSameFloor(r.Room);
			Room room1 = EasyInput<Room>.Select(roomsOnSameFloor, _cancel);		
			r.MergePairRoom = room1;
		}
	}
}
