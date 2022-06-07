using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Frontend.CLI.Model;

namespace HospitalIS.Backend.Controller
{
    internal class UserAccountController
    {
        public const int PruningGracePeriodInDays = 30;
        public const int AppointmentModificationsInGracePeriod = 5;
        public const int AppointmentCreationsInGracePeriod = 8;
        
        public enum AccountProperty
        {
            USERNAME,
            PASSWORD,
            FIRSTNAME,
            LASTNAME,
            GENDER
        }
        
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
                    if (ua.Blocked != UserAccount.BlockedBy.NONE) throw new InvalidLoginAttemptException("Account is blocked");
                    MedicalRecordController.AddNotifsIfNecessary(ua);
                    DaysOffRequestModel.ShowDeletedAppointments(ua);
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
                user.Blocked = UserAccount.BlockedBy.SYSTEM;
                throw new UserAccountForcefullyBlockedException(
                    $"Exceeded possible number of appointment creations ({AppointmentCreationsInGracePeriod}) for the last {PruningGracePeriodInDays} days");
            }

            if (user.AppointmentModifiedTimestamps.Count > AppointmentModificationsInGracePeriod)
            {
                user.Blocked = UserAccount.BlockedBy.SYSTEM;
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
        
        public static List<AccountProperty> GetAccountProperties()
        {
            return Enum.GetValues(typeof(AccountProperty)).Cast<AccountProperty>().ToList();
        }
        
        public static void CopyAccount(UserAccount target, UserAccount source, List<AccountProperty> whichProperties)
        {
            if (whichProperties.Contains(AccountProperty.USERNAME)) target.Username = source.Username;
            if (whichProperties.Contains(AccountProperty.PASSWORD)) target.Password = source.Password;
            if (whichProperties.Contains(AccountProperty.FIRSTNAME)) target.Person.FirstName = source.Person.FirstName;
            if (whichProperties.Contains(AccountProperty.LASTNAME)) target.Person.LastName = source.Person.LastName;
            if (whichProperties.Contains(AccountProperty.GENDER)) target.Person.Gender = source.Person.Gender;
        }
        
        public static List<UserAccount> GetModifiablePatientAccounts()
        {
            return IS.Instance.Hospital.UserAccounts.Where(account => !account.Deleted && account.Type == UserAccount.AccountType.PATIENT).ToList();
        }
        
        public static List<UserAccount> GetBlockedAccounts()
        {
            return IS.Instance.Hospital.UserAccounts.Where(account => !account.Deleted && IsBlocked(account)).ToList();
        }
        
        public static List<UserAccount> GetNotBlockedPatientAccounts()
        {
            return IS.Instance.Hospital.UserAccounts.Where(account => !account.Deleted && !IsBlocked(account) && account.Type == UserAccount.AccountType.PATIENT).ToList();
        }

        internal static bool IsBlocked(UserAccount account)
        {
            return account.Blocked != UserAccount.BlockedBy.NONE;
        }
    }
}
