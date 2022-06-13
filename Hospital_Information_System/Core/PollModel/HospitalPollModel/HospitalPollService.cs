using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.PollModel.HospitalPollModel
{
	public class HospitalPollService : IHospitalPollService
	{
		private readonly IHospitalPollRepository _repo;

		public HospitalPollService(IHospitalPollRepository repo)
		{
			_repo = repo;
		}

		public IEnumerable<HospitalPoll> GetAll()
		{
			return _repo.GetAll();
		}
	}
}
