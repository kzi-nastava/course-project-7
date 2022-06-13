using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.PollModel.AppointmentPollModel
{
	public class AppointmentPollService : IAppointmentPollService
	{
		private readonly IAppointmentPollRepository _repo;

		public AppointmentPollService(IAppointmentPollRepository repo)
		{
			_repo = repo;
		}

		public IEnumerable<AppointmentPoll> GetAll()
		{
			return _repo.GetAll();
		}
	}
}
