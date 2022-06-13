using System;
using System.Collections.Generic;
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
	}
}
