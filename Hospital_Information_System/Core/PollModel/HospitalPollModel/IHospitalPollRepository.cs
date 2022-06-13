using System;
using System.Collections.Generic;

namespace HIS.Core.PollModel.HospitalPollModel
{
	public interface IHospitalPollRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<HospitalPoll> GetAll();
		public HospitalPoll Get(int id);
		public HospitalPoll Add(HospitalPoll obj);
		public void Remove(HospitalPoll obj);
	}
}
