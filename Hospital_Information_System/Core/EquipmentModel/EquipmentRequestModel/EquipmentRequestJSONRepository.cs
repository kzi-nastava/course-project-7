using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace HIS.Core.EquipmentModel.EquipmentRequestModel
{
    public class EquipmentRequestJSONRepository : IEquipmentRequestRepository
    {
        private readonly IList<EquipmentRequest> _equipmentRequests;
        private readonly string _fname;
        private readonly JsonSerializerSettings _settings;

        public EquipmentRequestJSONRepository(string fname, JsonSerializerSettings settings)
        {
            _fname = fname;
            _settings = settings;
            _equipmentRequests = JsonConvert.DeserializeObject<List<EquipmentRequest>>(File.ReadAllText(fname), _settings);
        }

        public int GetNextId()
        {
            return _equipmentRequests.Count;
        }

        public IEnumerable<EquipmentRequest> GetAll()
        {
            return _equipmentRequests.Where(o => !o.Deleted);
        }

        public void Add(EquipmentRequest request)
        {
            _equipmentRequests.Add(request);
        }

        public void Save()
        {
            File.WriteAllText(_fname, JsonConvert.SerializeObject(_equipmentRequests, Formatting.Indented, _settings));
        }
    }
}