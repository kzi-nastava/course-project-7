using System;
using System.Collections.Generic;

namespace HIS.Core.ModificationRequestModel.DeleteRequestModel
{
	public interface IDeleteRequestRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<DeleteRequest> GetAll();
		public DeleteRequest Get(int id);
		public DeleteRequest Add(DeleteRequest obj);
		public void Remove(DeleteRequest obj);
	}
}
