using System.Collections.Generic;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel.ReferralModel
{
    public interface IReferralRepository
    {
        public int GetNextId();
        public void Save();
        public IEnumerable<Referral> GetAll();
        public Referral Add(Referral obj);
        public void Remove(Referral obj);
        public Referral Get(int id);
        IEnumerable<Referral> GetUnused();
    }
}