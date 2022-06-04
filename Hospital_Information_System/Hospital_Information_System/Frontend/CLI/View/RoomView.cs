using HospitalIS.Backend;
using HospitalIS.Backend.RoomModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Frontend.CLI.View
{
	internal class RoomView : View
	{
		private const string errFloorNonNegative = "Room floor must be a non-negative integer!";
		private const string errNameIsEmpty = "Name cannot be empty!";
		private readonly IRoomService _service;
		private readonly IEnumerable<RoomType> _types;
		private readonly IEnumerable<RoomProperty> _modifiableProperties;

		internal RoomView(IRoomService roomService)
		{
			_service = roomService;
			_types = Utility.GetEnumValues<RoomType>();
			_modifiableProperties = Utility.GetEnumValues<RoomProperty>().Except(new List<RoomProperty> { RoomProperty.EQUIPMENT });
		}

		#region commands
		internal void CmdRoomCreate()
		{
			Room r = InputRoom(Utility.GetEnumValues<RoomProperty>());
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
			return EasyInput<Room>.Select(_service.Get().ToList(), r => r.Name, _cancel);
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
				r.Name = InputName();
			}

			if (whichProperties.Contains(RoomProperty.TYPE))
			{
				r.Type = InputType();
			}

			if (whichProperties.Contains(RoomProperty.FLOOR))
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

		private RoomType InputType()
		{
			return EasyInput<RoomType>.Select(_types.Where(t => t != RoomType.WAREHOUSE).ToList(), _cancel);
		}
		#endregion
	}
}
