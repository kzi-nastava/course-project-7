using System;
using System.Collections.Generic;
using System.Text;
using HIS.Core.PersonModel.DoctorModel;

namespace HIS.Core.PersonModel.UserAccountModel
{
    public interface IUserAccountService
    {
        void Add(UserAccount account);
        void Remove(UserAccount account);
        void Update(UserAccount account, UserAccount updatedAccount, IEnumerable<AccountProperty> properties);
        IEnumerable<UserAccount> GetByUsername(string username);
        IEnumerable<UserAccount> GetModifiable(UserAccount user);
        IEnumerable<UserAccount> GetNotBlockedPatientAccounts();
        IEnumerable<UserAccount> GetBlockedPatientAccounts();
        IEnumerable<UserAccount> GetAll();
        UserAccount AttemptLogin(string username, string password);
        void AddModifiedAppointmentTimestamp(UserAccount user, DateTime timestamp);
        void AddCreatedAppointmentTimestamp(UserAccount user, DateTime timestamp);
        void DetectTrolling(UserAccount user);
        void PruneTimestamps(UserAccount user);
        public UserAccount GetUserFromDoctor(Doctor doctor);
    }
}
