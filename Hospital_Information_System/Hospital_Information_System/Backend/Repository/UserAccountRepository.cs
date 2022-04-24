using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend.Repository
{
    internal class UserAccountRepository : IRepository<UserAccount>
    {
        private static int pruningGracePeriodInDays = 30;
        private static int appointmentModificationsInGracePeriod = 5;
        private static int appointmentCreationsInGracePeriod = 8;
        public void Add(UserAccount entity)
        {
            List<UserAccount> UserAccounts = IS.Instance.Hospital.UserAccounts;

            entity.Id = UserAccounts.Count > 0 ? UserAccounts.Last().Id + 1 : 0;
            UserAccounts.Add(entity);
        }

        public UserAccount GetById(int id)
        {
            return IS.Instance.Hospital.UserAccounts.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.UserAccounts = JsonConvert.DeserializeObject<List<UserAccount>>(File.ReadAllText(fullFilename), settings);
            IS.Instance.Hospital.UserAccounts.ForEach(ua => PruneTimestamps(ua));
        }

        public void Remove(UserAccount entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<UserAccount, bool> condition)
        {
            IS.Instance.Hospital.UserAccounts.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.UserAccounts, Formatting.Indented, settings));
        }

        public void AddModifiedAppointmentTimestamp(UserAccount user, DateTime timestamp)
        {
            user.AppointmentModifiedTimestamps.Add(timestamp);
            DetectTrolling(user);
        }
        public void AddMadeAppointmentTimestamp(UserAccount user, DateTime timestamp)
        {
            user.AppointmentMadeTimestamps.Add(timestamp);
            DetectTrolling(user);
        }

        public void DetectTrolling(UserAccount user)
        {
            PruneTimestamps(user);

            if (user.Type != UserAccount.AccountType.PATIENT)
            {
                return;
            }

            if (user.AppointmentMadeTimestamps.Count > appointmentCreationsInGracePeriod)
            {
                user.Blocked = true;
                throw new UserAccountForcefullyBlockedException(
                    $"Exceeded possible number of appointment creations ({appointmentCreationsInGracePeriod}) for the last {pruningGracePeriodInDays} days");
            }

            if (user.AppointmentModifiedTimestamps.Count > appointmentModificationsInGracePeriod)
            {
                user.Blocked = true;
                throw new UserAccountForcefullyBlockedException(
                    $"Exceeded possible number of appointment modifications ({appointmentModificationsInGracePeriod}) for the last {pruningGracePeriodInDays} days");
            }
        }

        public void PruneTimestamps(UserAccount user)
        {
            void prune(List<DateTime> timestamps)
            {
                List<DateTime> prunableTimestamps = new List<DateTime>();
                timestamps.ForEach(t => { if ((DateTime.Now - t).TotalDays > pruningGracePeriodInDays) prunableTimestamps.Add(t); });
                prunableTimestamps.ForEach(t => timestamps.Remove(t));
            }

            prune(user.AppointmentMadeTimestamps);
            prune(user.AppointmentModifiedTimestamps);
        }
    }
}
