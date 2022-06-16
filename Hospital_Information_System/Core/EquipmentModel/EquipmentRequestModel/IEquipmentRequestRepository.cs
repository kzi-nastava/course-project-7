using System.Collections.Generic;

namespace HIS.Core.EquipmentModel.EquipmentRequestModel
{
    public interface IEquipmentRequestRepository
    {
        public void Save();

        public IEnumerable<EquipmentRequest> GetAll();
        public void Add(EquipmentRequest request);
        int GetNextId();
        
    }
}