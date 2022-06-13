using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.PersonModel.UserAccountModel
{
    public interface IUserAccountService
    {
        IEnumerable<UserAccount> GetAll();
        UserAccount AttemptLogin(string username, string password);
        void AddModifiedAppointmentTimestamp(UserAccount user, DateTime timestamp);
        void AddCreatedAppointmentTimestamp(UserAccount user, DateTime timestamp);
        void DetectTrolling(UserAccount user);
        void PruneTimestamps(UserAccount user);
    }
}
