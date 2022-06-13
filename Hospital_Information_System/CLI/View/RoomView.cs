using HIS.Core;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.RoomModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.CLI.View
{
	internal class RoomView : View
	{
		private const string errFloorNonNegative = "Room floor must be a non-negative integer!";
		private const string errNameIsEmpty = "Name cannot be empty!";

		private const string hintInputName = "Enter room name";
		private const string hintInputType = "Select room type";
		private const string hintInputFloor = "Enter room floor";

		private readonly IRoomService _service;
		private readonly IEnumerable<RoomType> _types;
		private readonly IEnumerable<RoomProperty> _modifiableProperties;

		internal RoomView(IRoomService roomService, UserAccount user) : base(user)
		{
			_service = roomService;
			_types = Utility.GetEnumValues<RoomType>();
			_modifiableProperties = Utility.GetEnumValues<RoomProperty>().Except(new List<RoomProperty> { RoomProperty.EQUIPMENT });
		}

		#region commands
		internal Room CmdRoomCreate()
		{
			Room r = InputRoom();
			_service.Add(r);
			return r;
		}

		internal void CmdRoomView()
		{
			Room room = Select();
			Print(room.ToString());
		}

		internal void CmdRoomUpdate()
		{
			Room room = SelectModifiable();
			var whichProperties = SelectProperties();
			Room src = InputRoom(whichProperties);
			_service.Copy(src, room, whichProperties);
		}

		internal void CmdRoomDelete()
		{
			Room room = SelectModifiable();
			_service.Remove(room);
		}
		#endregion

		#region helper
		internal Room InputRoom(int floor)
		{
			var room = InputRoom(Utility.GetEnumValues<RoomProperty>().Except(new List<RoomProperty> { RoomProperty.FLOOR }));
			room.Floor = floor;
			return room;
		}

		private Room InputRoom()
		{
			return InputRoom(Utility.GetEnumValues<RoomProperty>());
		}

		private Room Select()
		{
			return EasyInput<Room>.Select(_service.GetAll().ToList(), r => r.Name, _cancel);
		}

		private Room SelectModifiable()
		{
			return EasyInput<Room>.Select(_service.GetModifiable().ToList(), r => r.Name, _cancel);
		}

		private IList<RoomProperty> SelectProperties()
		{
			return EasyInput<RoomProperty>.SelectMultiple(_modifiableProperties.ToList(), _cancel);
		}

		private Room InputRoom(IEnumerable<RoomProperty> whichProperties)
		{
			Room r = new Room();

			if (whichProperties.Contains(RoomProperty.NAME))
			{
				Hint(hintInputName);
				r.Name = InputName();
			}

			if (whichProperties.Contains(RoomProperty.TYPE))
			{
				Hint(hintInputType);
				r.Type = InputType();
			}

			if (whichProperties.Contains(RoomProperty.FLOOR))
			{
				Hint(hintInputFloor);
				r.Floor = InputFloor();
			}

			return r;
		}

		private string InputName()
		{
			return EasyInput<string>.Get(
				new List<Func<string, bool>> { s => s.Length != 0 },
				new[] { errNameIsEmpty },
				_cancel
			);
		}

		private int InputFloor()
		{
			return EasyInput<int>.Get(
				new List<Func<int, bool>> { n => n >= 0 },
				new[] { errFloorNonNegative },
				_cancel
			);
		}

		private RoomType InputType()
		{
			return EasyInput<RoomType>.Select(_types.Where(t => t != RoomType.WAREHOUSE).ToList(), _cancel);
		}
		#endregion
	}
}
