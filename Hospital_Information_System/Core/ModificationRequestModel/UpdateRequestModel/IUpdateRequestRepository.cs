using System;
using System.Collections.Generic;

namespace HIS.Core.ModificationRequestModel.UpdateRequestModel
{
	public interface IUpdateRequestRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<UpdateRequest> GetAll();
		public UpdateRequest Get(int id);
		public UpdateRequest Add(UpdateRequest obj);
		public void Remove(UpdateRequest obj);
	}
}
