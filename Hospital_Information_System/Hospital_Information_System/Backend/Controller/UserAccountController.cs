using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Controller
{
    internal class UserAccountController
    {
        public const int PruningGracePeriodInDays = 30;
        public const int AppointmentModificationsInGracePeriod = 5;
        public const int AppointmentCreationsInGracePeriod = 8;

        public class InvalidLoginAttemptException : Exception
        {
            public InvalidLoginAttemptException(string errorMessage) : base($"Login failed: {errorMessage}.")
            {

            }
        }

        public static List<UserAccount> GetModifiableAccounts()
        {
            return IS.Instance.Hospital.UserAccounts.Where(u => !u.Deleted).ToList();
        }

        public static UserAccount AttemptLogin(string username, string password)
        {
            foreach (UserAccount ua in GetModifiableAccounts())
            {
                if (ua.Username == username && ua.Password == password)
                {
                    if (ua.Blocked) throw new InvalidLoginAttemptException("Account is blocked");
                    return ua;
                }
            }

            throw new InvalidLoginAttemptException("Invalid credentials");
        }

        public static void AddModifiedAppointmentTimestamp(UserAccount user, DateTime timestamp)
        {
            user.AppointmentModifiedTimestamps.Add(timestamp);
            DetectTrolling(user);
        }
        public static void AddCreatedAppointmentTimestamp(UserAccount user, DateTime timestamp)
        {
            user.AppointmentCreatedTimestamps.Add(timestamp);
            DetectTrolling(user);
        }

        public static void DetectTrolling(UserAccount user)
        {
            PruneTimestamps(user);

            if (user.Type != UserAccount.AccountType.PATIENT)
            {
                return;
            }

            if (user.AppointmentCreatedTimestamps.Count > AppointmentCreationsInGracePeriod)
            {
                user.Blocked = true;
                throw new UserAccountForcefullyBlockedException(
                    $"Exceeded possible number of appointment creations ({AppointmentCreationsInGracePeriod}) for the last {PruningGracePeriodInDays} days");
            }

            if (user.AppointmentModifiedTimestamps.Count > AppointmentModificationsInGracePeriod)
            {
                user.Blocked = true;
                throw new UserAccountForcefullyBlockedException(
                    $"Exceeded possible number of appointment modifications ({AppointmentModificationsInGracePeriod}) for the last {PruningGracePeriodInDays} days");
            }
        }

        public static void PruneTimestamps(UserAccount user)
        {
            void prune(List<DateTime> timestamps)
            {
                List<DateTime> prunableTimestamps = new List<DateTime>();
                timestamps.ForEach(t => { if ((DateTime.Now - t).TotalDays > PruningGracePeriodInDays) prunableTimestamps.Add(t); });
                prunableTimestamps.ForEach(t => timestamps.Remove(t));
            }

            prune(user.AppointmentCreatedTimestamps);
            prune(user.AppointmentModifiedTimestamps);
        }
    }
}
