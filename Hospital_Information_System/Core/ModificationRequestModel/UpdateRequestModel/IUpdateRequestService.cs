using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.ModificationRequestModel.UpdateRequestModel
{
    public interface IUpdateRequestService
    {
        IEnumerable<UpdateRequest> GetAll();
        UpdateRequest Add(UpdateRequest request);
        List<UpdateRequest> GetPending();
        bool IsModifiable(UpdateRequest request);
    }
}
