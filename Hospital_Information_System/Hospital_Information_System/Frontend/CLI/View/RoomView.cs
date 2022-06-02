using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend;
using HospitalIS.Backend.Room;

namespace HospitalIS.Frontend.CLI.View
{
	internal class RoomView : View
	{
		private const string errFloorNonNegative = "Room floor must be a non-negative integer!";
		private const string errNameIsEmpty = "Name cannot be empty!";
		private readonly RoomService _service;
		private readonly IEnumerable<Room.RoomType> _types;
		private readonly IEnumerable<Room.RoomProperty> _properties;

		internal RoomView(RoomService roomService)
		{
			_service = roomService;
			_types = Utility.GetEnumValues<Room.RoomType>();
			_properties = Utility.GetEnumValues<Room.RoomProperty>();
		}

#region commands
		internal void CmdRoomCreate()
		{
			Room r = InputRoom(Utility.GetEnumValues<Room.RoomProperty>());
			_service.Add(r);
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

		private Room Select()
		{
			return EasyInput<Room>.Select(_service.Get(), r => r.Name, _cancel);
		}

		private Room SelectModifiable()
		{
			return EasyInput<Room>.Select(_service.GetModifiable(), r => r.Name, _cancel);
		}

		private IList<Room.RoomProperty> SelectProperties()
		{
			return EasyInput<Room.RoomProperty>.SelectMultiple(_properties.ToList(), _cancel);
		}

		private Room InputRoom(IList<Room.RoomProperty> whichProperties)
		{
			Room r = new Room();

			if (whichProperties.Contains(Room.RoomProperty.NAME))
			{
				r.Name = InputName();
			}

			if (whichProperties.Contains(Room.RoomProperty.TYPE))
			{
				r.Type = InputType();
			}

			if (whichProperties.Contains(Room.RoomProperty.FLOOR))
			{
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

		private Room.RoomType InputType()
		{
			return EasyInput<Room.RoomType>.Select(_types.Where(t => t != Room.RoomType.WAREHOUSE).ToList(), _cancel);
		}
#endregion
	}
}
