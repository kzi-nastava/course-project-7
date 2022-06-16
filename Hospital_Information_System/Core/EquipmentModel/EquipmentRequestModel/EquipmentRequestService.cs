using System.Collections.Generic;
using HIS.Core.Foundation;
using HIS.Core.RoomModel;

namespace HIS.Core.EquipmentModel.EquipmentRequestModel
{
    public class EquipmentRequestService : IEquipmentRequestService
    {
        private readonly IEquipmentRequestRepository _repo;
        private readonly IRoomService _roomService;
        private readonly TaskQueue _taskQueue;

        public EquipmentRequestService(IEquipmentRequestRepository repo,IRoomService roomService, TaskQueue taskQueue)
        {
            _repo = repo;
            _roomService = roomService;
            _taskQueue = taskQueue;
            
            ContinueUnfinishedTasks();
        }

        public IEnumerable<EquipmentRequest> GetAll()
        {
            return _repo.GetAll();
        }

        public void Add(EquipmentRequest request)
        {
            request.Id = _repo.GetNextId();
            _repo.Add(request);
            AddToTasks(request);
        }
        
        #region Order tasks
        private void Perform(EquipmentRequest request)
        {
            if (request.Added)
            {
                return;
            }
            foreach (var equipment in request.Equipment)
            {
                _roomService.Move(equipment.Key, equipment.Value, null, _roomService.GetWarehouse());    
            }
            
            System.Console.WriteLine("EquipmentRequestService.cs: Finished order.");
            request.Added = true;
        }

        private void ContinueUnfinishedTasks()
        {
            foreach (var pendingRelocation in _repo.GetAll())
            {
                AddToTasks(pendingRelocation);
            }
        }
        private void AddToTasks(EquipmentRequest e)
        {
            _taskQueue.Add(() => Perform(e), e.OrderTime.AddDays(1));
        }
        #endregion
    }
}