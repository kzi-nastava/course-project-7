using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend.Repository
{
    internal class UserAccountRepository : IRepository<UserAccount>
    {
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
    }
}
