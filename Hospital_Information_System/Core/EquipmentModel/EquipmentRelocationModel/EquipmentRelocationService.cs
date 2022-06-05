using HIS.Core.Foundation;
using System.Linq;

namespace HIS.Core.EquipmentModel.EquipmentRelocationModel
{
	public class EquipmentRelocationService : IEquipmentRelocationService
	{
		private readonly IEquipmentRelocationRepository _repo;
		private readonly TaskQueue _taskQueue;

		public EquipmentRelocationService(IEquipmentRelocationRepository repo, TaskQueue taskQueue)
		{
			_repo = repo;
			_taskQueue = taskQueue;

			ContinueUnfinishedTasks();
		}

		public void Add(EquipmentRelocation e)
		{
			e.Id = (_repo.Get().LastOrDefault()?.Id ?? -1) + 1;
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
				return;

			int amount = e.RoomFrom.Equipment[e.Equipment];

			if (e.RoomTo.Equipment.ContainsKey(e.Equipment))
			{
				e.RoomTo.Equipment[e.Equipment] += amount;
			}
			else
			{
				e.RoomTo.Equipment[e.Equipment] = amount;
			}

			e.RoomFrom.Equipment[e.Equipment] -= amount;

			System.Console.WriteLine(e.ToString());
			Remove(e);
		}

		private void ContinueUnfinishedTasks()
		{
			foreach (var pendingRelocation in _repo.Get())
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
