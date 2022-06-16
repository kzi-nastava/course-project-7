using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.ModificationRequestModel.UpdateRequestModel
{
	public class UpdateRequestService : IUpdateRequestService
	{
		private readonly IUpdateRequestRepository _repo;

		public UpdateRequestService(IUpdateRequestRepository repo)
		{
			_repo = repo;
		}

        public IEnumerable<UpdateRequest> GetAll()
		{
			return _repo.GetAll();
		}

		public UpdateRequest Add(UpdateRequest request)
		{
			return _repo.Add(request);
		}
		
		public List<UpdateRequest> GetPending()
		{
			return _repo.GetAll().Where(request => !request.Deleted && IsModifiable(request)).ToList();
		}

		public bool IsModifiable(UpdateRequest request)
		{
			return request.OldAppointment.ScheduledFor > DateTime.Now && request.State == ModificationRequest.StateType.PENDING;	
		}
		
		
		
		
	}
}
