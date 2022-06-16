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
        public List<DaysOffRequest> GetChanged(UserAccount user);

        public List<DaysOffRequest> Get(Doctor doctor);

        public List<DaysOffRequest> GetFuture();

        public List<DaysOffRequest> GetApproved(Doctor doctor);

        public List<DaysOffRequest> GetSentAndApproved(Doctor doctor);
    }
}