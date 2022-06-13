using HIS.Core.PersonModel.DoctorModel;
using System;
using System.Collections.Generic;

namespace HIS.Core.PollModel.AppointmentPollModel
{
	public interface IAppointmentPollRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<AppointmentPoll> GetAll();
		public IEnumerable<AppointmentPoll> GetAll(Doctor doctor);
		public AppointmentPoll Get(int id);
		public AppointmentPoll Add(AppointmentPoll obj);
		public void Remove(AppointmentPoll obj);
	}
}
