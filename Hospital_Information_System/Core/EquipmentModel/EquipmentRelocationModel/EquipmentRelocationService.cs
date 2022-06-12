using HIS.Core.Foundation;
using HIS.Core.RoomModel;
using System.Linq;

namespace HIS.Core.EquipmentModel.EquipmentRelocationModel
{
	public class EquipmentRelocationService : IEquipmentRelocationService
	{
		private readonly IEquipmentRelocationRepository _repo;
		private readonly IRoomService _roomService;
		private readonly TaskQueue _taskQueue;

		public EquipmentRelocationService(IEquipmentRelocationRepository repo, IRoomService roomService, TaskQueue taskQueue)
		{
			_repo = repo;
			_roomService = roomService;
			_taskQueue = taskQueue;

			ContinueUnfinishedTasks();
		}

		public void Add(EquipmentRelocation e)
		{
			e.Id = _repo.GetNextId();
			_repo.Add(e);
			AddToTasks(e);
		}

		public void Remove(EquipmentRelocation e)
		{
			_repo.Remove(e);
		}

		#region Relocation tasks
		private void Perform(EquipmentRelocation e)
		{
			if (e.Deleted)
			{
				return;
			}

			if (e.RoomFrom.Deleted)
			{
				Remove(e);
				return;
			}

			if (e.RoomTo.Deleted)
			{
				e.RoomTo = _roomService.GetWarehouse();
			}

			_roomService.Move(e.Equipment, e.RoomFrom.Equipment[e.Equipment], e.RoomFrom, e.RoomTo);

			System.Console.WriteLine("EquipmentRelocationService.cs: Finished relocation.");
			Remove(e);
		}

		private void ContinueUnfinishedTasks()
		{
			foreach (var pendingRelocation in _repo.GetAll())
			{
				AddToTasks(pendingRelocation);
			}
		}

		private void AddToTasks(EquipmentRelocation e)
		{
			_taskQueue.Add(() => Perform(e), e.When);
		}
		#endregion
	}
}
