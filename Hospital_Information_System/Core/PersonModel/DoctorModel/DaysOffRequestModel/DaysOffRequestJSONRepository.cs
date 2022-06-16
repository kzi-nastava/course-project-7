using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace HIS.Core.PersonModel.DoctorModel.DaysOffRequestModel
{
    public class DaysOffRequestJSONRepository : IDaysOffRequestRepository
    {
        private readonly IList<DaysOffRequest> _daysOffRequests;
        private readonly string _fname;
        private readonly JsonSerializerSettings _settings;

        public DaysOffRequestJSONRepository(string fname, JsonSerializerSettings settings)
        {
            _fname = fname;
            _settings = settings;
            DaysOffRequestJSONReferenceConverter.Repo = this;
            _daysOffRequests = JsonConvert.DeserializeObject<List<DaysOffRequest>>(File.ReadAllText(fname), _settings);
        }

        public int GetNextId()
        {
            return _daysOffRequests.Count;
        }

        public IEnumerable<DaysOffRequest> GetAll()
        {
            return _daysOffRequests.Where(o => !o.Deleted);
        }

        public DaysOffRequest Get(int id)
        {
            return _daysOffRequests.FirstOrDefault(r => r.Id == id);
        }

        public DaysOffRequest Add(DaysOffRequest obj)
        {
            obj.Id = GetNextId();
            _daysOffRequests.Add(obj);
            return obj;
        }

        public void Remove(DaysOffRequest obj)
        {
            obj.Deleted = true;
        }

        public void Save()
        {
            File.WriteAllText(_fname, JsonConvert.SerializeObject(_daysOffRequests, Formatting.Indented, _settings));
        }
        
        public List<DaysOffRequest> GetSentDaysOffRequests()
        {
            return _daysOffRequests.Where(a => !a.Deleted && a.State == DaysOffRequest.DaysOffRequestState.SENT).ToList();
        }

        public List<DaysOffRequest> GetDaysOffRequests(Doctor doctor)
        {
            return _daysOffRequests.Where(a => !a.Deleted && a.Requester == doctor).ToList();
        }

        public List<DaysOffRequest> GetFutureDaysOffRequests()
        {
            return _daysOffRequests.Where(a => !a.Deleted && a.State == DaysOffRequest.DaysOffRequestState.APPROVED && DateTime.Compare(DateTime.Now, a.Start) <= 0).ToList();
        }

        public List<DaysOffRequest> GetApprovedRequests(Doctor doctor)
        {
            return _daysOffRequests.Where(a =>
                !a.Deleted && a.Requester == doctor && a.State == DaysOffRequest.DaysOffRequestState.APPROVED).ToList();
        }

        public List<DaysOffRequest> GetSentAndApprovedRequests(Doctor doctor)
        {
            return _daysOffRequests.Where(a =>
                !a.Deleted && a.Requester == doctor && 
                (a.State == DaysOffRequest.DaysOffRequestState.APPROVED || a.State == DaysOffRequest.DaysOffRequestState.SENT)).ToList();
        }
    }
}