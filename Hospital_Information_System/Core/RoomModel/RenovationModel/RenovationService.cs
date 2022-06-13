using HIS.Core.Foundation;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.RoomModel.RenovationModel
{
	public class RenovationService : IRenovationService
	{
		private IRenovationRepository _repo;
		private IRoomService _roomService;
		private TaskQueue _taskQueue;

		public RenovationService(IRenovationRepository renovationRepository, TaskQueue taskQueue, IRoomService roomService)
		{
			_repo = renovationRepository;
			_taskQueue = taskQueue;
			ContinueUnfinishedTasks();
			_roomService = roomService;
		}

		public Renovation Add(Renovation r)
		{
			_repo.Add(r);
			AddToTasks(r);
			return r;
		}

		public void Remove(Renovation r)
		{
			_repo.Remove(r);
		}

		public IEnumerable<Renovation> GetAll()
		{
			return _repo.GetAll();
		}

		public IEnumerable<Renovation> GetAll(Room r)
		{
			return _repo.Get(r);
		}

		public bool IsRenovating(Room room, DateTime start, DateTime end)
		{
			return _repo.GetAll().Count(ren =>
				ren.Room == room &&
				new DateTimeRange(start, end).Intersects(ren.TimeRange)) > 0;
		}

		#region tasks
		private void Perform(Renovation e)
		{
			if (e.Deleted)
			{
				return;
			}

			if (e.Room.Deleted)
			{
				Remove(e);
				return;
			}

			if (e.IsMerging() && e.MergePairRoom.Deleted)
			{
				Remove(e);
				return;
			}

			if (e.IsSplitting())
			{
				Split(e);
			}
			else if (e.IsMerging())
			{
				Merge(e);
			}

			Console.WriteLine("RenovationService.cs: Finished renovation.");
			Remove(e);
		}

		private void Split(Renovation e)
		{
			_roomService.Add(e.SplitNewRoom1);
			_roomService.Add(e.SplitNewRoom2);

			foreach (var kv in e.EquipmentForSplitNewRoom1)
			{
				int amountRoom2 = e.Room.Equipment[kv.Key] - kv.Value;
				_roomService.Move(kv.Key, kv.Value, e.Room, e.SplitNewRoom1);
				_roomService.Move(kv.Key, amountRoom2, e.Room, e.SplitNewRoom2);
			}

			_roomService.Remove(e.Room);
		}

		private void Merge(Renovation e)
		{
			for (int i = 0; i < e.MergePairRoom.Equipment.Count; i++)
			{
				var kv = e.MergePairRoom.Equipment.ElementAt(i);
				_roomService.Move(kv.Key, kv.Value, e.MergePairRoom, e.Room);
			}

			_roomService.Remove(e.MergePairRoom);
		}

		private void ContinueUnfinishedTasks()
		{
			foreach (Renovation pendingRenovation in _repo.GetAll())
			{
				AddToTasks(pendingRenovation);
			}
		}

		private void AddToTasks(Renovation e)
		{
			_taskQueue.Add(() => Perform(e), e.TimeRange.End);
		}
        #endregion
    }
}
