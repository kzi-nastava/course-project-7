using System;
using System.Collections.Generic;
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
	}
}
