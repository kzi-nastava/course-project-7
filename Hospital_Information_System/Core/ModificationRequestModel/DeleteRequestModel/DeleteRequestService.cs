using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.ModificationRequestModel.DeleteRequestModel
{
	public class DeleteRequestService : IDeleteRequestService
	{
		private readonly IDeleteRequestRepository _repo;

		public DeleteRequestService(IDeleteRequestRepository repo)
		{
			_repo = repo;
		}

        public IEnumerable<DeleteRequest> GetAll()
		{
			return _repo.GetAll();
		}

		public DeleteRequest Add(DeleteRequest request)
		{
			return _repo.Add(request);
		}

		public void Remove(DeleteRequest request)
		{
			_repo.Remove(request);
		}
		
		public IEnumerable<DeleteRequest> GetPending()
		{
			return _repo.GetAll().Where(request => !request.Deleted && IsModifiable(request)).ToList();
		}
		
		public bool IsModifiable(DeleteRequest request)
		{
			return request.Appointment.ScheduledFor > DateTime.Now && request.State == ModificationRequest.StateType.PENDING;
		}
	}
}
