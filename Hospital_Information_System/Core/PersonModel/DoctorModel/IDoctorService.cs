using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.DoctorModel
{
    public interface IDoctorService
    {
        IEnumerable<Doctor> GetAll();
    }
}
