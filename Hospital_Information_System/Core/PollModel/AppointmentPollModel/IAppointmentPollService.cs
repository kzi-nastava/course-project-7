using HIS.Core.PersonModel.DoctorModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.PollModel.AppointmentPollModel
{
    public interface IAppointmentPollService
    {
        IEnumerable<AppointmentPoll> GetAll();
        IEnumerable<AppointmentPoll> GetAll(Doctor doctor);
    }
}
