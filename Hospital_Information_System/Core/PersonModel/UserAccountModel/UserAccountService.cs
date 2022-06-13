using System;
using System.Collections.Generic;
using HIS.Core.PersonModel.UserAccountModel.Util;

namespace HIS.Core.PersonModel.UserAccountModel
{
	public class UserAccountService : IUserAccountService
	{
		private readonly IUserAccountRepository _repo;

		public UserAccountService(IUserAccountRepository repo)
		{
			_repo = repo;
		}

        public IEnumerable<UserAccount> GetAll()
        {
            return _repo.GetAll();
        }

        public UserAccount AttemptLogin(string username, string password)
        {
            foreach (UserAccount ua in GetAll())
            {
                if (ua.Username == username && ua.Password == password)
                {
                    if (ua.Blocked != UserAccount.BlockedBy.NONE) throw new InvalidLoginAttemptException("Account is blocked");
                    // TODO: Implement.
                    //MedicalRecordController.AddNotifsIfNecessary(ua);
                    return ua;
                }
            }

            throw new InvalidLoginAttemptException("Invalid credentials");
        }

        public void AddCreatedAppointmentTimestamp(UserAccount user, DateTime timestamp)
        {
            user.AppointmentCreatedTimestamps.Add(timestamp);
            DetectTrolling(user);
        }

        public void AddModifiedAppointmentTimestamp(UserAccount user, DateTime timestamp)
        {
            user.AppointmentModifiedTimestamps.Add(timestamp);
            DetectTrolling(user);
        }

        public void DetectTrolling(UserAccount user)
        {
            PruneTimestamps(user);

            if (user.Type != UserAccount.AccountType.PATIENT)
            {
                return;
            }

            if (user.AppointmentCreatedTimestamps.Count > UserAccountConstants.AppointmentCreationsInGracePeriod)
            {
                user.Blocked = UserAccount.BlockedBy.SYSTEM;
                throw new UserAccountForcefullyBlockedException(
                    $"Exceeded possible number of appointment creations ({UserAccountConstants.AppointmentCreationsInGracePeriod}) for the last {UserAccountConstants.PruningGracePeriodInDays} days");
            }

            if (user.AppointmentModifiedTimestamps.Count > UserAccountConstants.AppointmentModificationsInGracePeriod)
            {
                user.Blocked = UserAccount.BlockedBy.SYSTEM;
                throw new UserAccountForcefullyBlockedException(
                    $"Exceeded possible number of appointment modifications ({UserAccountConstants.AppointmentModificationsInGracePeriod}) for the last {UserAccountConstants.PruningGracePeriodInDays} days");
            }
        }

        public void PruneTimestamps(UserAccount user)
        {
            void prune(List<DateTime> timestamps)
            {
                List<DateTime> prunableTimestamps = new List<DateTime>();
                timestamps.ForEach(t => { if ((DateTime.Now - t).TotalDays > UserAccountConstants.PruningGracePeriodInDays) prunableTimestamps.Add(t); });
                prunableTimestamps.ForEach(t => timestamps.Remove(t));
            }

            prune(user.AppointmentCreatedTimestamps);
            prune(user.AppointmentModifiedTimestamps);
        }
    }
}
