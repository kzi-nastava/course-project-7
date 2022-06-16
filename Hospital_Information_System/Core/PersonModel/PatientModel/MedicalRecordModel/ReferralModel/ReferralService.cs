using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.AppointmentModel;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.Util;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel.ReferralModel
{
    public class ReferralService : IReferralService
    {
        private readonly IReferralRepository _repo;
        
        public ReferralService(IReferralRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Referral> GetAll()
        {
            return _repo.GetAll();
        }
        
        public int GetNextId()
        {
            return _repo.GetNextId();
        }

        public void Save()
        {
            _repo.Save();
        }

        public Referral Get(int id)
        {
            return _repo.Get(id);
        }

        public Referral Add(Referral obj)
        {
            return _repo.Add(obj);
        }

        public void Remove(Referral obj)
        {
            _repo.Remove(obj);
        }
        
        public List<ReferralProperty> GetAllReferralProperties()
        {
            return Enum.GetValues(typeof(ReferralProperty)).Cast<ReferralProperty>().ToList();
        }
    }
}