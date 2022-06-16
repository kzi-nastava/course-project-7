using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel.ReferralModel
{
    public class ReferralJSONRepository : IReferralRepository
    {
        private readonly IList<Referral> _referrals;
        private readonly string _fname;
        private readonly JsonSerializerSettings _settings;

        public ReferralJSONRepository(string fname, JsonSerializerSettings settings)
        {
            _fname = fname;
            _settings = settings;
            ReferralJSONReferenceConverter.Repo = this;
            _referrals = JsonConvert.DeserializeObject<List<Referral>>(File.ReadAllText(fname), _settings);
        }

        public int GetNextId()
        {
            return _referrals.Count;
        }

        public IEnumerable<Referral> GetAll()
        {
            return _referrals.Where(o => !o.Deleted);
        }

        public Referral Get(int id)
        {
            return _referrals.FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<Referral> GetUnused()
        {
            return _referrals.Where(r => !r.Scheduled && !r.Deleted);
        }

        public Referral Add(Referral obj)
        {
            obj.Id = GetNextId();
            _referrals.Add(obj);
            return obj;
        }

        public void Remove(Referral obj)
        {
            obj.Deleted = true;
        }

        public void Save()
        {
            File.WriteAllText(_fname, JsonConvert.SerializeObject(_referrals, Formatting.Indented, _settings));
        }
    }
}