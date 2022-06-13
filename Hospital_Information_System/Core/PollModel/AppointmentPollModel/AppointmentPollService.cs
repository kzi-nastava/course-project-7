using HIS.Core.PersonModel.DoctorModel;
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

        public AppointmentPoll Add(AppointmentPoll poll)
        {
			return _repo.Add(poll);
        }

        public IEnumerable<AppointmentPoll> GetAll()
		{
			return _repo.GetAll();
		}

        public IEnumerable<AppointmentPoll> GetAll(Doctor doctor)
        {
			return _repo.GetAll(doctor);
        }
    }
}
