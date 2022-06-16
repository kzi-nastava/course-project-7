using HIS.Core.PersonModel.DoctorModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.PollModel.AppointmentPollModel
{
    public interface IAppointmentPollService
    {
        AppointmentPoll Add(AppointmentPoll poll);
        IEnumerable<AppointmentPoll> GetAll();
        IEnumerable<AppointmentPoll> GetAll(Doctor doctor);
        Dictionary<Doctor, IList<AppointmentPoll>> GetAppointmentPollsByDoctor();
        IEnumerable<KeyValuePair<Doctor, double>> GetTotalAverageRatingsByDoctor();
    }
}
