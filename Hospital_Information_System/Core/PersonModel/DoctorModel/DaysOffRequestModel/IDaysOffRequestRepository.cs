using System;
using System.Collections.Generic;
using HIS.Core.PersonModel.UserAccountModel;

namespace HIS.Core.PersonModel.DoctorModel.DaysOffRequestModel
{
    public interface IDaysOffRequestRepository
    {
        public int GetNextId();
        public void Save();
        public IEnumerable<DaysOffRequest> GetAll();
        public DaysOffRequest Get(int id);
        public DaysOffRequest Add(DaysOffRequest obj);
        public void Remove(DaysOffRequest obj);

        public List<DaysOffRequest> GetSent();
        List<DaysOffRequest> GetChanged(UserAccount user);

        public List<DaysOffRequest> GetDaysOffRequests(Doctor doctor);

        public List<DaysOffRequest> GetFutureDaysOffRequests();

        public List<DaysOffRequest> GetApprovedRequests(Doctor doctor);

        public List<DaysOffRequest> GetSentAndApprovedRequests(Doctor doctor);
    }
}