using HIS.Core.PersonModel.PatientModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.PollModel.HospitalPollModel
{
    public interface IHospitalPollService
    {
        IEnumerable<HospitalPoll> GetAll();
        IEnumerable<HospitalPoll> GetAll(Patient pollee);
    }
}
